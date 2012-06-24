﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using AnotherJiraRestClient;

namespace JiraIssueBrowser.Controllers
{
    public class Util
    {
        public const string KEY_JIRA_ACCOUNT = "JiraAccount";
        public const string KEY_JIRA_CLIENT = "JiraClient";
        private const string VIRTUAL_PATH_JIRA_ACCOUNT_XML = "~/App_Data/jira_account.xml";

        /// <summary>
        /// Returns the JiraClient for this application. The JiraClient
        /// is stored in cache. If the cache has not been set this method
        /// will load the JiraAccount by loading JiraAccount from xml and
        /// constructing a new JiraClient.
        /// </summary>
        /// <param name="context">Context that contains the cache</param>
        /// <param name="server">Used to get absolute file path of the xml file</param>
        /// <returns>the JiraClient for this application</returns>
        public static JiraClient GetJiraClient(HttpContextBase context, HttpServerUtilityBase server)
        {
            return GetFromCache<JiraClient>(
                KEY_JIRA_CLIENT,
                () => new JiraClient(
                    GetJiraAccountFromXml(
                    server.MapPath(VIRTUAL_PATH_JIRA_ACCOUNT_XML))),
                context);
        }

        public static T GetFromCache<T>(string key, Func<T> loadData, HttpContextBase context)
        {
            T obj = (T)context.Cache.Get(key);
            if (obj == null)
            {
                obj = loadData();
                context.Cache.Insert(key, obj);
            }
            return obj;
        }

        private static JiraAccount GetJiraAccountFromXml(string path)
        {
            var serializer = new XmlSerializer(typeof(JiraAccount));
            FileStream stream = new FileStream(path, FileMode.Open);
            var account = (JiraAccount) serializer.Deserialize(stream);
            stream.Close();
            return account;
        }
    }
}