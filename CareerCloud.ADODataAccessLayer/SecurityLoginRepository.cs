using System;
using CareerCloud.Pocos;
using CareerCloud.DataAccessLayer;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Linq;

namespace CareerCloud.ADODataAccessLayer
{
    public class SecurityLoginRepository : IDataRepository<SecurityLoginPoco>
    {
        private string _connStr;
        public SecurityLoginRepository()
        {
            var config = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            config.AddJsonFile(path, false);
            var root = config.Build();
            _connStr = root.GetSection("ConnectionStrings").GetSection("DataConnection").Value;
        }
        public void Add(params SecurityLoginPoco[] items)
        {
            using (SqlConnection connection = new SqlConnection(_connStr))
            {
                foreach (SecurityLoginPoco poco in items)
                {
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = connection;
                    comm.CommandText = @"INSERT INTO [dbo].[Security_Logins]
                                           ([Id]
                                           ,[Login]
                                           ,[Password]
                                           ,[Created_Date]
                                           ,[Password_Update_Date]
                                           ,[Agreement_Accepted_Date]
                                           ,[Is_Locked]
                                           ,[Is_Inactive]
                                           ,[Email_Address]
                                           ,[Phone_Number]
                                           ,[Full_Name]
                                           ,[Force_Change_Password]
                                           ,[Prefferred_Language])
                                     VALUES
                                           (@Id,
                                            @Login,
                                            @Password,
                                            @Created_Date,
                                            @Password_Update_Date,
                                            @Agreement_Accepted_Date,
                                            @Is_Locked,
                                            @Is_Inactive,
                                            @Email_Address,
                                            @Phone_Number,
                                            @Full_Name,
                                            @Force_Change_Password,
                                            @Preferred_Language)";
                    comm.Parameters.AddWithValue("@Id", poco.Id);
                    comm.Parameters.AddWithValue("@Login", poco.Login);
                    comm.Parameters.AddWithValue("@Password", poco.Password);
                    comm.Parameters.AddWithValue("@Created_Date", poco.Created);
                    comm.Parameters.AddWithValue("@Password_Update_Date", poco.PasswordUpdate);
                    comm.Parameters.AddWithValue("@Agreement_Accepted_Date", poco.AgreementAccepted);
                    comm.Parameters.AddWithValue("@Is_Locked", poco.IsLocked);
                    comm.Parameters.AddWithValue("@Is_Inactive", poco.IsInactive);
                    comm.Parameters.AddWithValue("@Email_Address", poco.EmailAddress);
                    comm.Parameters.AddWithValue("@Phone_Number", poco.PhoneNumber);
                    comm.Parameters.AddWithValue("@Full_Name", poco.FullName);
                    comm.Parameters.AddWithValue("@Force_Change_Password", poco.ForceChangePassword);
                    comm.Parameters.AddWithValue("@Preferred_Language", poco.PrefferredLanguage);



                    connection.Open();
                    int rowAffected = comm.ExecuteNonQuery();
                    connection.Close();

                }
            }
        }

        public void CallStoredProc(string name, params Tuple<string, string>[] parameters)
        {
            throw new NotImplementedException();
        }

        public IList<SecurityLoginPoco> GetAll(params Expression<Func<SecurityLoginPoco, object>>[] navigationProperties)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                    cmd.CommandText = @"SELECT [Id]
                                      ,[Login]
                                      ,[Password]
                                      ,[Created_Date]
                                      ,[Password_Update_Date]
                                      ,[Agreement_Accepted_Date]
                                      ,[Is_Locked]
                                      ,[Is_Inactive]
                                      ,[Email_Address]
                                      ,[Phone_Number]
                                      ,[Full_Name]
                                      ,[Force_Change_Password]
                                      ,[Prefferred_Language]
                                      ,[Time_Stamp]
                                  FROM [dbo].[Security_Logins]";
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                SecurityLoginPoco[] pocos = new SecurityLoginPoco[500];
                int index = 0;
                while (reader.Read())
                {
                    SecurityLoginPoco poco = new SecurityLoginPoco();
                    poco.Id = reader.GetGuid(0);
                 
                    poco.Login = (string)reader["Login"];
                    poco.Password = (string)reader["Password"];
                    poco.Created = reader.GetDateTime(3);
                    if (!reader.IsDBNull(4))
                        poco.PasswordUpdate = reader.GetDateTime(4);
                    else
                        poco.PasswordUpdate = null;
                    if (!reader.IsDBNull(5))
                        poco.AgreementAccepted = reader.GetDateTime(5);
                    else
                        poco.AgreementAccepted = null;
                    poco.IsLocked = reader.GetBoolean(6);
                    poco.IsInactive = reader.GetBoolean(7);
                    poco.EmailAddress = (string)reader["Email_Address"];
                    if (!reader.IsDBNull(9))
                        poco.PhoneNumber = (string)reader["Phone_Number"];
                  
                    else
                        poco.PhoneNumber = null;
                    poco.FullName = (string)reader["Full_Name"];
                    poco.ForceChangePassword = reader.GetBoolean(11);
                    if (!reader.IsDBNull(12))
                        poco.PrefferredLanguage = (string)reader["Prefferred_Language"];
                    else
                        poco.PrefferredLanguage = null;
                    pocos[index] = poco;
                    index++;
                }
                conn.Close();
                return pocos.Where(a => a != null).ToList();
            }
        }

        public IList<SecurityLoginPoco> GetList(Expression<Func<SecurityLoginPoco, bool>> where, params Expression<Func<SecurityLoginPoco, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public SecurityLoginPoco GetSingle(Expression<Func<SecurityLoginPoco, bool>> where, params Expression<Func<SecurityLoginPoco, object>>[] navigationProperties)
        {
            IQueryable<SecurityLoginPoco> pocos = GetAll().AsQueryable();
            return pocos.Where(where).FirstOrDefault();
        }

        public void Remove(params SecurityLoginPoco[] items)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                foreach (SecurityLoginPoco poco in items)
                {
                    cmd.CommandText = @"DELETE Security_Logins
                                        where ID = @Id";
                    cmd.Parameters.AddWithValue("@Id", poco.Id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public void Update(params SecurityLoginPoco[] items)
        {
            using (SqlConnection connection = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                foreach (var poco in items)
                {
                    cmd.CommandText = @"UPDATE [dbo].[Security_Logins]
                                       SET [Id] = @Id
                                          ,[Login] = @Login
                                          ,[Password] = @Password
                                          ,[Created_Date] = @Created_Date
                                          ,[Password_Update_Date] = @Password_Update_date
                                          ,[Agreement_Accepted_Date] = @Agreement_Accepted_Date
                                          ,[Is_Locked] = @Is_Locked
                                          ,[Is_Inactive] = @Is_Inactive
                                          ,[Email_Address] = @Email_Address
                                          ,[Phone_Number] = @Phone_Number
                                          ,[Full_Name] = @Full_Name
                                          ,[Force_Change_Password] = @Force_Change_Password
                                          ,[Prefferred_Language] = @Prefferred_Language
                                     WHERE [Id] = @Id";
                    cmd.Parameters.AddWithValue("@Id", poco.Id);
                    cmd.Parameters.AddWithValue("@Login", poco.Login);
                    cmd.Parameters.AddWithValue("@Password", poco.Password);
                    cmd.Parameters.AddWithValue("@Created_Date", poco.Created);
                    cmd.Parameters.AddWithValue("@Password_Update_Date", poco.PasswordUpdate);
                    cmd.Parameters.AddWithValue("@Agreement_Accepted_Date", poco.AgreementAccepted);
                    cmd.Parameters.AddWithValue("@Is_Locked", poco.IsLocked);
                    cmd.Parameters.AddWithValue("@Is_Inactive", poco.IsInactive);
                    cmd.Parameters.AddWithValue("@Email_Address", poco.EmailAddress);
                    cmd.Parameters.AddWithValue("@Phone_Number", poco.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Full_Name", poco.FullName);
                    cmd.Parameters.AddWithValue("@Force_Change_Password", poco.ForceChangePassword);
                    cmd.Parameters.AddWithValue("@Prefferred_Language", poco.PrefferredLanguage);

                    connection.Open();
                    int count = cmd.ExecuteNonQuery();
                    if (count <=0 )
                    {
                        throw new Exception();
                    }
                    connection.Close();
                }
            }
        }
    }
}
