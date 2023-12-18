namespace acomba.zuper_api.AcombaServices
{
    public interface IAcombaConnection
    {
        void OpenConnection();
        void CloseConnection();
    }
    public class AcombaConnection :IAcombaConnection
    {
        private readonly IConfiguration _configuration;
        private AcoSDK.AcoSDKX AcoSDKInt = new AcoSDK.AcoSDKX();
        private AcoSDK.AcombaX Acomba = new AcoSDK.AcombaX();
        private AcoSDK.User UserInt = new AcoSDK.User();
        public AcombaConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OpenConnection()
        {
           

            int Version;
            string CompanyPath;
            string AcombaPath;
            string MotDePasse;
            int Exist, Error;

            // Obtenir la version la plus récente du SDK
            Version = AcoSDKInt.VaVersionSDK;

            // Démarrer le SDK avec la version obtenue
            Error = AcoSDKInt.Start(Version);

            // Si le SDK est bien démarré
            if (Error == 0)
            {
                // Chemin d'accès de la société à ouvrir
                CompanyPath = _configuration["CompanyPath"]; //"C:\\F1000.dta\\DemoSDK_EN";

                // Chemin d'accès des cartes d'enregistrement d'Acomba
                AcombaPath = _configuration["AcombaPath"]; //"C:\\Aco_SDK";

                // Mot de passe de l'usager
                MotDePasse = _configuration["Password"];//"DEMO";

                // Vérification de l'existence de la société à ouvrir
                Exist = Acomba.CompanyExists(CompanyPath);

                if (Exist != 0)
                {
                    // Ouverture de la société Demo
                    Error = Acomba.OpenCompany(AcombaPath, CompanyPath);

                    if (Error == 0)
                    {
                        // Recherche de l'usager "supervisor" pour trouver son CardPos
                        UserInt.PKey_UsNumber = _configuration["Pkey"];
                        Error = UserInt.FindKey(1, false);

                        if (Error == 0)
                        {
                            // Connexion de l'usager "supervisor" avec son mot de passe

                            Error = Acomba.LogCurrentUser(UserInt.Key_UsCardPos, MotDePasse);

                            if (Error == 0)
                            {

                                Console.WriteLine("Connexion de l'usager complétée avec succès.");
                                
                            }
                            else
                            {
                                
                                Console.WriteLine("Erreur: " + Acomba.GetErrorMessage(Error));
                            }
                        }
                        else
                        {
                            
                            Console.WriteLine("Erreur: " + Acomba.GetErrorMessage(Error));
                        }
                    }
                    else
                    {
                       
                        Console.WriteLine("Erreur: " + Acomba.GetErrorMessage(Error));
                    }
                }
                else
                {
                   
                    Console.WriteLine("Dossier de la société invalide");
                }
            }
            else
            {
                
                Console.WriteLine("Erreur: " + Acomba.GetErrorMessage(Error));
            }
        }
        public void CloseConnection()
        {
            int Error;
            Error = Acomba.CloseCompany();
            if(Error == 0)
            {
                Console.WriteLine("Company closure successfully completed");
            }
            else
            {
                Console.WriteLine("Error:" + Acomba.GetErrorMessage(Error));
            }
        }
    }
}
