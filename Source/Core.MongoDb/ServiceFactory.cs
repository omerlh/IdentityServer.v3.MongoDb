﻿using MongoDB.Bson;
using MongoDB.Driver;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Core.MongoDb
{
    public class ServiceFactory : IdentityServerServiceFactory
    {
        public ServiceFactory(Registration<IUserService> userService)
            : this("mongodb://localhost", userService)
        { }

        public ServiceFactory(string mongoUrl, Registration<IUserService> userService)
            : this(mongoUrl, userService, DefaultStoreSettings())
        {}

        public ServiceFactory(string mongoUrl, Registration<IUserService> userService, StoreSettings settings)
            : this(DefaultSettings(mongoUrl),
            DefaultStoreSettings(), userService)
        {

        }
        private ServiceFactory(MongoClientSettings settings, StoreSettings storeSettings, Registration<IUserService> userService)
        {
            var client = new MongoClient(settings);
            var server = client.GetServer();
            var db = server.GetDatabase(storeSettings.Database);
            UserService = userService;
            ClientStore =
                Registration.RegisterSingleton<IClientStore>(new ClientStore(db, storeSettings.ClientCollection));
            ScopeStore = Registration.RegisterSingleton<IScopeStore>(new ScopeStore(db, storeSettings.ScopeCollection));
        }

        private static MongoClientSettings DefaultSettings(string mongoUrl)
        {
            var settings = MongoClientSettings.FromUrl(MongoUrl.Create(mongoUrl));
            settings.GuidRepresentation = GuidRepresentation.Standard;
            return settings;
        }

        public static StoreSettings DefaultStoreSettings()
        {
            return new StoreSettings
            {
                Database = "identityserver",
                ClientCollection = "clients"
            };
        }

    }


}
