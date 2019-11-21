using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Diagnostics;


namespace NormeRadnika
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=192.168.0.3;Initial Catalog=FeroApp;User ID=sa;Password=AdminFX9.";
            string connectionStringr = @"Data Source=192.168.0.3;Initial Catalog=RFIND;User ID=sa;Password=AdminFX9.";
            DateTime dat1 = DateTime.Now.AddDays(-1); ;
            DateTime dat10 = DateTime.Now.AddDays(-1); ;
            string mm1 = "";
        
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var first = month.AddMonths(-1);
            var last = month.AddDays(-1);
            int zadnjiDan = last.Day;
            int mjesec = first.Month;
            int godina = first.Year;


            for (int dan1 = 1; dan1<=zadnjiDan; dan1++)
            {
                
                dat1 = new DateTime(godina, mjesec, dan1 );
              

                if (dat1.Month <= 9)
                    mm1 = "0";
                string dats = dat1.Year.ToString() + '-' + mm1 + dat1.Month.ToString() + '-' + dat1.Day.ToString();  // današnji datum
                string dats2 = dat1.Day.ToString() + '.' + mm1 + dat1.Month.ToString() +'.' + dat1.Year.ToString() ;  // današnji datum
                string id1, firma1, ime1, opis1;

                using (SqlConnection cnr = new SqlConnection(connectionStringr))
                {
                    cnr.Open();
                    string sql1r = "delete from rfind.dbo.PSRodsustva where datum='" + dats + "'";
                    SqlCommand sqlCommand2 = new SqlCommand(sql1r, cnr);
                    SqlDataReader reader2 = sqlCommand2.ExecuteReader();

                    cnr.Close();
                }

                //// odsustva
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    cn.Open();
                    string sql1 = "rfind.dbo.PSR_odsustva '" + dats + "',1";
                    SqlCommand sqlCommand = new SqlCommand(sql1, cn);

                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        id1 = (reader["radnikid"].ToString());
                        firma1 = (reader["firma"].ToString());
                        ime1 = (reader["ime"].ToString());
                        opis1 = (reader["danpsr"].ToString());


                        using (SqlConnection cnr = new SqlConnection(connectionStringr))
                        {
                            cnr.Open();
                            string sql1r = "insert into rfind.dbo.PSRodsustva(DATUM,ID,idfirme,opis,ime) values('" + dats + "'," + id1 + "," + firma1 + ",'" + opis1 + "','" + ime1 + "')";
                            SqlCommand sqlCommand2 = new SqlCommand(sql1r, cnr);
                            SqlDataReader reader2 = sqlCommand2.ExecuteReader();

                            cnr.Close();
                        }
                    }
                }

                // prisustva

                using (SqlConnection cnr = new SqlConnection(connectionStringr))
                {
                    cnr.Open();
                    string sql1r = "delete from rfind.dbo.PSRprisustva where datum='" + dats + "'";
                    SqlCommand sqlCommand2 = new SqlCommand(sql1r, cnr);
                    SqlDataReader reader2 = sqlCommand2.ExecuteReader();

                    cnr.Close();
                }


                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    cn.Open();
                    string sql1 = "rfind.dbo.PSR_odsustva '" + dats + "',0";
                    SqlCommand sqlCommand = new SqlCommand(sql1, cn);
                    int bsati = 0;
                    string opis0 = "";
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        bsati = 0;
                        id1 = (reader["radnikid"].ToString());
                        firma1 = (reader["firma"].ToString());
                        ime1 = (reader["ime"].ToString());
                        opis1 = (reader["danpsr"].ToString());
                        opis0 = opis1;
                        int day1 = (int)dat1.DayOfWeek;
                        int d1     = dat1.Month;
                        int bsatt = 7;  // pon-pet =7

                        if (day1 == 0 || day1 == 6)
                            bsatt = 5;   // subota 5

                        if (opis1.Contains(':'))
                            {
                            string[] o1 = opis1.Split(':');
                            opis1 = o1[0];
                            opis1 = opis1.Replace("p", "");
                            opis1 = opis1.Replace("j", "");
                            opis1 = opis1.Replace("n", "");
                            bsati = int.Parse(opis1);
                            bsati = bsatt + bsati;

                            opis1 = o1[1];

                            opis1 = opis1.Replace("p", "");
                            opis1 = opis1.Replace("j", "");
                            opis1 = opis1.Replace("n", "");                            
                            bsati = bsati + bsatt + int.Parse(opis1);

                        }
                        else
                        {

                            if (opis1.Contains('e') || opis1.Contains('b') || opis1.TrimEnd().Length==0)
                            {
                                bsati = 0;
                            }
                            else if ( opis1.Contains('y') || opis1.Contains('l') || opis1.Contains('g'))
                            {
                                bsati = 8;
                            }
                            else
                            {
                                opis1 = opis1.Replace("p", "");
                                opis1 = opis1.Replace("j", "");
                                opis1 = opis1.Replace("n", "");

                                bsati = int.Parse(opis1);

                                bsati = bsatt + bsati;
                            }

                        }



                        using (SqlConnection cnr = new SqlConnection(connectionStringr))
                        {
                            cnr.Open();
                            string sql1r = "insert into rfind.dbo.PSRprisustva(DATUM,ID,idfirme,opis,ime,sati) values('" + dats + "'," + id1 + "," + firma1 + ",'" + opis0 + "','" + ime1 + "',"+bsati.ToString()+")";
                            SqlCommand sqlCommand2 = new SqlCommand(sql1r, cnr);
                            SqlDataReader reader2 = sqlCommand2.ExecuteReader();

                            cnr.Close();
                        }
                    }
                }



            }

        }
    }
}