using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                case "First Name":
                    return "Enter First Name";
                case "Last Name":
                    return "Enter Last Name";
                case "Email":
                    return "Enter Email";
                case "Email Pattern":
                    return @"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$";
                default:
                    throw new ArgumentException("Invalid field name");
            }
        }
    }
}
