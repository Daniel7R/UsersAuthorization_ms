namespace UsersAuthorization.Application.Queues
{
    public static class Queues
    {
        //consumed in notifications
        public const string GET_USER_BY_ID = "user.by_id";
        public const string ALL_USER_EMAILS = "users.emails";
        public const string USERS_BULK_INFO = "users.bulk.info";
    }
}
