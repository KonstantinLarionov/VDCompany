#!/bin/bash

if ! [ -x "$(command -v docker compose)" ]; then
  echo 'Error: docker compose is not installed.' >&2
  exit 1
fi

domains=(companyvd.ru www.companyvd.ru)
rsa_key_size=4096
data_path="./certbot"
email="afc.studio@yandex.ru" # Adding a valid address is strongly recommended
staging=0 # Set to 1 if you're testing your setup to avoid hitting request limits

path="/etc/letsencrypt/live/$domains"

docker compose up -d certbot

docker compose exec -it certbot sh -c "test -f /etc/letsencrypt/live/$domains/fullchain.pem"
file_existsFullchain=$?
echo $file_existsFullchain
docker compose exec -it certbot sh -c "test -f /etc/letsencrypt/live/$domains/privkey.pem"
file_existsPrivkey=$?
echo $file_existsPrivkey
docker compose exec -it certbot sh -c "test -f /etc/letsencrypt/live/$domains/chain.pem"
file_existsChain=$?
echo $file_existsChain
docker compose exec -it certbot sh -c "test -f /etc/letsencrypt/live/$domains/cert.pem"
file_existsCert=$?
echo $file_existsCert
docker compose stop certbot

if [ $file_existsFullchain -eq 0 ]; then
  echo "Copying file"
  docker cp container_id:/path/to/$1 ./$1
fi


if [ $file_existsFullchain -eq 0 ] && [ $file_existsPrivkey -eq 0 ] && [ $file_existsChain -eq 0 ] && [ $file_existsCert -eq 0 ]; then
  echo "Existing data found for $domains. Continue upping docker compose"
  exit 0
fi


if [ ! -e "$data_path/conf/options-ssl-nginx.conf" ] || [ ! -e "$data_path/conf/ssl-dhparams.pem" ]; then
  echo "### Downloading recommended TLS parameters ..."
  mkdir -p "$data_path/conf"
  curl -s https://raw.githubusercontent.com/certbot/certbot/master/certbot-nginx/certbot_nginx/_internal/tls_configs/options-ssl-nginx.conf > "$data_path/conf/options-ssl-nginx.conf"
  curl -s https://raw.githubusercontent.com/certbot/certbot/master/certbot/certbot/ssl-dhparams.pem > "$data_path/conf/ssl-dhparams.pem"
  echo
fi

echo "### Creating dummy certificate for $domains ..."
mkdir -p "$data_path/conf/live/$domains"

docker compose -f docker-compose.yml run --rm --entrypoint "mkdir -p /etc/letsencrypt/live/$domains" certbot 

docker compose run --rm --entrypoint "\
  openssl req -x509 -nodes -newkey rsa:$rsa_key_size -days 1\
    -keyout '$path/privkey.pem' \
    -out '$path/fullchain.pem' \
    -subj '/CN=localhost'" certbot
echo


echo "### Starting nginx ..."
docker compose up --force-recreate -d nginx
echo

echo "### Deleting dummy certificate for $domains ..."
docker compose run --rm --entrypoint "\
  rm -Rf /etc/letsencrypt/live/$domains && \
  rm -Rf /etc/letsencrypt/archive/$domains && \
  rm -Rf /etc/letsencrypt/renewal/$domains.conf" certbot
echo


echo "### Requesting Let's Encrypt certificate for $domains ..."
#Join $domains to -d args
domain_args=""
for domain in "${domains[@]}"; do
  domain_args="$domain_args -d $domain"
done

# Select appropriate email arg
#case "$email" in
#  "") 
email_arg="--register-unsafely-without-email"
#  *) email_arg="--email $email" ;;
#esac

# Enable staging mode if needed
if [ $staging != "0" ]; then staging_arg="--staging"; fi

docker compose run --rm --entrypoint "\
  certbot certonly --webroot -w /var/www/certbot \
    $staging_arg \
    $email_arg \
    $domain_args \
    --rsa-key-size $rsa_key_size \
    --agree-tos \
    --noninteractive \
    --force-renewal" certbot
echo

echo "### Reloading nginx ..."
# docker compose exec nginx nginx -s reload

