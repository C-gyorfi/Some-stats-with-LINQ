using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTest

{
    /*this project is to read data from a CSV file to get some stats. 
     * The data format in the CSV files is the following: {Name}, {Date}, {Amount}, {Charity} and the first row is a header */

    public class DonationStats
    {
        public static double CalculateAverageDailyDonations(string filePath)
        {
            // Average per day of fundraising 
            var csvData = File.ReadAllText(filePath).ToLines().Skip(1).Where(x => x.HasValue());
            var donationList = csvData.Select(x => new Donation { Donor = x.Split(',')[0], Date = x.Split(',')[1].To<DateTime>(), Amount = x.Split(',')[2].Remove(0, 1).To<double>(), Charity = x.Split(',')[3] });
            var grouByDate = donationList.GroupBy(x => x.Date, (key, group) => new { dDate = key, amounts = group.Select(x => x.Amount) });
            var result = grouByDate.Average(x => x.amounts.Sum());
            Console.WriteLine(result);
            return result;
        }

        static DateTime BestFundraisingDay(string filePath)
        {
            // Day of highest total donations 
            var csvData = File.ReadAllText(filePath).ToLines().Skip(1).Where(x => x.HasValue());
            var donationList = csvData.Select(x => new Donation { Donor = x.Split(',')[0], Date = x.Split(',')[1].To<DateTime>(), Amount = x.Split(',')[2].Remove(0, 1).To<double>(), Charity = x.Split(',')[3] });
            var grouByDate = donationList.GroupBy(x => x.Date, (key, group) => new { dDate = key, amounts = group.Select(x => x.Amount) });
            var bestResult = grouByDate.Select(x => x.amounts.Sum()).Max();
            var bestDay = grouByDate.Where(x => x.amounts.Sum() == bestResult).Select(x => x.dDate).First();
            Console.WriteLine(bestDay);
            return bestDay;
        }

        static void MostGenerousDonors(string filePath, string OutputFile)
        {
            // Create a CSV file to contain one row for each unique donor with the following columns:
            // {Name}, {Total Donations}, {Favourite Charity}, {Most generous day}
            // Total donations: is the sum of all donations made to all charities by that person.
            // Favourite charity: is the charity which has received the most total donations from that person.
            // Most generous day: is the day in which that person has donated more to charities in total.
            // Note: The records should be sorted based on the most generous donor to the least one (i.e. Total Donations).

            var csvData = File.ReadAllText(filePath).ToLines().Skip(1).Where(x => x.HasValue());
            var donationList = csvData.Select(x => new Donation { Donor = x.Split(',')[0], Date = x.Split(',')[1].To<DateTime>(), Amount = x.Split(',')[2].Remove(0, 1).To<double>(), Charity = x.Split(',')[3] });
            var allnames = donationList.Select(x => x.Donor).Distinct();
            var bestDonors = new List<Donation>();

            foreach (var name in allnames)
            {

                var sumOfAmounts = donationList.Where(x => x.Donor == name).Sum(x => x.Amount);

                var groupByCharity = donationList.Where(x => x.Donor == name).GroupBy(x => x.Charity, (key, group) => new { cKey = key, amounts = group.Sum(x => x.Amount) });
                var favouriteCharity = groupByCharity.OrderByDescending(x => x.amounts).Select(x => x.cKey).First();

                var groupByDate = donationList.Where(x => x.Donor == name).GroupBy(x => x.Date, (key, group) => new { dKey = key, amounts = group.Sum(x => x.Amount) });
                var bestDay = groupByDate.OrderByDescending(x => x.amounts).Select(x => x.dKey).First();

                bestDonors.Add(new Donation { Donor = name, Amount = sumOfAmounts, Charity = favouriteCharity, Date = bestDay });

            }

            var result = bestDonors.OrderByDescending(x => x.Amount).Select(x => "{0}, {1}, {2}, {3}".FormatWith(x.Donor, x.Amount, x.Charity, x.Date)).ToLinesString();

            Console.Write(result);
            File.WriteAllText(OutputFile, result);
        }
    }

    //
    public class Donation
    {
        public string Donor { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string Charity { get; set; }
    }
}
