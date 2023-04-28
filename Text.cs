using System;

namespace Tube_Traveller
{
    public static class Text
    {
        public static string Get(string field)
        {
            switch (field)
            {
                case "Login":
                    return "Enter Username or Email";
                case "Username":
                    return "Enter Username";
                case "Password":
                    return "Enter Password";
                case "Email":
                    return "Enter Email";
                default:
                    throw new ArgumentException("Invalid field name");
            }
        }

        private const string emailPattern = @"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$";
        public static string GetEmailPattern() => emailPattern;


        private const string RouteMethod_R = "Regular";
        public static string GetRouteMethod_R() => RouteMethod_R;


        private const string RouteMethod_A = "Algorithmic";
        public static string GetRouteMethod_A() => RouteMethod_A;
    }
}
