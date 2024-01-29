namespace SwappyBot.EntityFramework
{
    using System;
    using Microsoft.EntityFrameworkCore;

    public static class Db
    {
        public static readonly MySqlServerVersion Version = new(new Version(8, 0, 34));
    }
}