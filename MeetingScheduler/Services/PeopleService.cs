using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.DirectoryServices;
using MeetingScheduler.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MeetingScheduler.Services
{
    public class LdapSettings
    {
        public string Domain { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class PeopleService
    {
        private string samAccount = "sAMAccountName";
        private string mail = "mail";
        private string cn = "cn";
        private string _domain;
        private string _userName;
        private string _password;
        //public List<PeopleService>();
        public Dictionary<string, User> allPeople = new Dictionary<string, User>();

        public PeopleService(IOptions<LdapSettings> ldap)
        {
            _domain = ldap.Value.Domain;
            _userName = ldap.Value.UserName;
            _password = ldap.Value.Password;
            FillDictionary();
        }

        protected void FillDictionary()
        {
            //User user = new User();
            //user.Id = "t.testov";
            //user.EmailAddress = "t.testov@redware.ru";
            //user.CN = "name1 Lastname1";
            //allPeople.Add(user.Id, user);
            //User user2 = new User();
            //user2.Id = "p.petrov";
            //user2.EmailAddress = "p.petrov@redware.ru";
            //user2.CN = "petr petrov";
            //allPeople.Add(user2.Id, user2);




            DirectoryEntry searchRoot = new DirectoryEntry(_domain);
            searchRoot.Username = _userName;
            searchRoot.Password = _password;
            using (DirectorySearcher directorySearcher = new DirectorySearcher(searchRoot))
            {
                directorySearcher.Filter = "(&(objectCategory=person)(objectClass=user))";
                directorySearcher.PropertiesToLoad.Add(samAccount);
                directorySearcher.PropertiesToLoad.Add(mail);
                directorySearcher.PropertiesToLoad.Add(cn);

                using (SearchResultCollection searchResultCollection = directorySearcher.FindAll())
                {
                    foreach (SearchResult searchResult in searchResultCollection)
                    {
                        User newUser = new User();
                        if (searchResult.Properties[samAccount].Count > 0)
                            newUser.Id = searchResult.Properties[samAccount][0].ToString();

                        if (searchResult.Properties[mail].Count > 0)
                            newUser.EmailAddress = searchResult.Properties[mail][0].ToString();

                        if (searchResult.Properties[cn].Count > 0)
                            newUser.CN = searchResult.Properties[cn][0].ToString();

                        allPeople.Add(newUser.Id, newUser);
                    }
                }
            }
        }
        public List<User> GetAllUser()
        {


            return allPeople.Values.ToList();
        }
    }
    
}
