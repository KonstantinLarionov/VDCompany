namespace VDCompany.Models
{
    public static class Conf
    {
        public static string ConnectDb { get; private set; } = "Data Source=db;Database=vdcompany;User ID=admin;Password=5@bcl0G5";
    }
    public static class MessagesUser
    {
        public static string MessageCaseOk { get; private set; } = "Ваше дело передано на рассмотрение. Мы свяжемся с Вами в ближайшее время";
        public static string MessageCaseFail { get; private set; } = "Ваше дело не создано, возможны проблемы с авторизацией, обновите страницу!";
    }
}
