//listmembers listdocs chatcontent
var isEnter = false;
var isCtrl = false;
window.onload = function(){
    if(window.matchMedia('(max-width: 991px)').matches) {
      $('#chatcontent').addClass('mobile contentright contentleft');
      $('#listdocs').addClass('toggled-right').css('width', '100%');
      $('#listmembers').addClass('toggled-left').css('width', '100%');
    }
  };
$('media').on({
   'hover': $('delete').css('dispay', 'block') 
});
$('#buttondocs').on({
    'click': toggleright });

$('#buttonmembers').on({
    'click': toggleleft });

function toggleright(){
    event.preventDefault();
    if(!$('#chatcontent').hasClass('mobile')){
    $('#chatcontent').toggleClass('contentright');
    }
    $('#listdocs').toggleClass('toggled-right');
    
}
function toggleleft(){
    event.preventDefault();
    if(!$('#chatcontent').hasClass('mobile')){
    $('#chatcontent').toggleClass('contentleft');
    }
    $('#listmembers').toggleClass('toggled-left');
}
$('#message').keyup(function(e){
    if(e.which == 13) isEnter=false;
    if(e.which == 17) isCtrl=false;
});
$('#message').keydown(function(e){
    if(e.which == 13) isEnter=true;
    if(e.which == 17) isCtrl=true;
    if(isCtrl && isEnter) send();
});
function send(){
    console.log($('#message').val());
    $('#message').val('');
}