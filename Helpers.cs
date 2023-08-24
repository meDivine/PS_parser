using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PS_parser
{
    public class Helpers
    {

        public string editionEdit(string edition)
        {
            if (edition is null || edition.Contains("PS4") || edition.Contains("PS5") )
            {
                edition = "";
            }
            return edition;
        }

        public string getPercentsDiscount(string textDiscount)
        {
            if (textDiscount != null)
            {
                string pattern = @"Save\s\d+%";
                Match match = Regex.Match(textDiscount, pattern);
                return match.Value.Trim();
            }
            else return "";
        }

        public string discountTwoTrim(string discountTwo)
        {
            if (discountTwo != null)
            {
                if (discountTwo.Contains("Subscribe to PlayStation Plus Extra to access this game and hundreds more in the Game Catalogue"))
                {
                    discountTwo = "PS Plus";
                }
                else if (discountTwo.Contains("Save"))
                {
                    string pattern = @"\d+%";
                    Match match = Regex.Match(discountTwo, pattern);
                    discountTwo = match.Value.Trim();
                }
                return discountTwo;
            }
            else return "";
        }
        /*
        public string[] splitFirstPrice(string price)
        {
            var delimiter = "TL";
            string[] resultFirstPrice = new string[2];

            if (price.Contains("Not available for purchase"))
            {
                resultFirstPrice[0] = "Not available for purchase";
                resultFirstPrice[1] = "Not available for purchase";
            }
            else if ((!price.Contains("Free") && !price.Contains("Trial") && !price.Contains("Included") && !price.Contains("Save") && 
                !price.Contains("Demo")) || price.Contains("TL"))
            {
                string[] splitPriceOne = price.Split(new string[] { delimiter }, 2, StringSplitOptions.None);
                resultFirstPrice[0] = splitPriceOne[0];
                resultFirstPrice[1] = splitPriceOne.Length > 1 ? splitPriceOne[1] : "";
            }
            else
            {
                resultFirstPrice[0] = "";
                resultFirstPrice[1] = "";
            }

            return resultFirstPrice;
        }*/

        public string[] splitFirstPrice(string? price)
        {
            var delimiter = "TL";
            string[] resultFirstPrice = new string[2];
            if (price != null && price.Contains("TL"))
            {
                string[] splitPriceOne = price.Split(new string[] { delimiter }, 2, StringSplitOptions.None);
                resultFirstPrice[0] = splitPriceOne[0];
                resultFirstPrice[1] = splitPriceOne.Length > 1 ? splitPriceOne[1] : "";
            }
            else
            {
                resultFirstPrice[0] = "";
                resultFirstPrice[1] = "";
            }

            return resultFirstPrice;
        }

        /*public string[] slitSecondPrice(string price)
        {
            var delimiter = "TL";
            string[] resultSecondPrice = new string[2];


            if ((!price.Contains("Free") && !price.Contains("Included") && !price.Contains("Add to Wishlist") && 
                !price.Contains("PlayStation Plus Exclusive") && !price.Contains("Learn More") && price != "" && 
                !price.Contains("Game Trial") && !price.Contains("Trial") && !price.Contains("trial") && 
                !price.Contains("Included") && !price.Contains("Save") && !price.Contains("Demo") && !price.Contains("access this game")) || price.Contains("TL"))
            {
                string[] splitPriceTwo = price.Split(new string[] { delimiter }, 2, StringSplitOptions.None);
                resultSecondPrice[0] = splitPriceTwo[0];
                resultSecondPrice[1] = splitPriceTwo[1].Length > 1 ? splitPriceTwo[1].Replace(" TL", "") : "";
            }
            else
            {
                resultSecondPrice[0] = "";
                resultSecondPrice[1] = "";
            }

            return resultSecondPrice;
        }*/

        public string[] slitSecondPrice(string price)
        {
            var delimiter = "TL";
            string[] resultSecondPrice = new string[2];


            if ( price != null && price.Contains("TL"))
            {
                string[] splitPriceTwo = price.Split(new string[] { delimiter }, 2, StringSplitOptions.None);
                resultSecondPrice[0] = splitPriceTwo[0];
                resultSecondPrice[1] = splitPriceTwo[1].Length > 1 ? splitPriceTwo[1].Replace(" TL", "") : "";
            }
            else
            {
                resultSecondPrice[0] = "";
                resultSecondPrice[1] = "";
            }

            return resultSecondPrice;
        }
    }
}
