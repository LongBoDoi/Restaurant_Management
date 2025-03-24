using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.ML.Common
{
    public class CommonValue
    {
        public static TokenValidationParameters TokenValidationParameters
        {
            get
            {
                return new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "ml_issuer",
                    ValidAudience = "ml_audience",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("da3d80f0-b79b-4179-b791-cc4e8bfdeb2b"))
                };
            }
        }

        public static List<string> CustomerScreenSettingKeys {
            get
            {
                return [
                    "IntroImageUrl",
                    "RestaurantName",
                    "RestaurantAddress",
                    "RestaurantPhoneNumber",
                    "SocialMediaUrls",
                    "OpeningTimes",
                    "RestaurantSlogan",
                    "DisplayMenuScreenForCustomer",
                    "DisplayMenuScreenForCustomerType",
                    "DisplayMenuScreenByItemsForCustomerType",
                    "ListMenuScreenForCustomerImages",
                    "ListMenuScreenForCustomerItems",
                    "DisplayBookingScreenForCustomer"
                ];
            }
        }
    }
}
