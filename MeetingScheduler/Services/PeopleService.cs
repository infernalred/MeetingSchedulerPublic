using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.DirectoryServices;
using MeetingScheduler.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.DirectoryServices.Protocols;
using System.Net;
using System.DirectoryServices.AccountManagement;

namespace MeetingScheduler.Services
{
    public class LdapSettings
    {
        public string Ldap { get; set; }
        public string Domain { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AdminGroup { get; set; }
        public string ManagerGroup { get; set; }
    }
    public class PeopleService
    {
        private readonly string samAccount = "sAMAccountName";
        private readonly string mail = "mail";
        private readonly string cn = "cn";
        private readonly string _ldap;
        private readonly string _domain;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _adminGroup;
        private readonly string _managerGroup;
        public Dictionary<string, User> allPeople = new Dictionary<string, User>();

        public PeopleService(IOptions<LdapSettings> ldap)
        {
            _ldap = ldap.Value.Ldap;
            _domain = ldap.Value.Domain;
            _userName = ldap.Value.UserName;
            _password = ldap.Value.Password;
            _adminGroup = ldap.Value.AdminGroup;
            _managerGroup = ldap.Value.ManagerGroup;
        }

        public void FillDictionary()
        {
            DirectoryEntry searchRoot = new DirectoryEntry(_ldap);
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

        public UserManager Login(string username, string password)
        {
            UserManager userLogin = null;
            using (var context = new PrincipalContext(ContextType.Domain, _domain, _userName, _password))
            {
                if (context.ValidateCredentials(username, password))
                {
                    var principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);
                    userLogin = new UserManager();
                    userLogin.Id = principal.SamAccountName;
                    userLogin.Email = principal.EmailAddress;
                    Role role = new Role();
                    role.Id = 1;
                    role.Name = "User";
                    if (principal.IsMemberOf(context, IdentityType.Name, _managerGroup) == true)
                    {
                        role.Name = "Manager";
                    }
                    if (principal.IsMemberOf(context, IdentityType.Name, _adminGroup) == true)
                    {
                        role.Name = "Admin";
                    }
                    userLogin.Role = role;
                    role.Users.Add(userLogin);
                }
                return userLogin;
            }
        }
    }

}
