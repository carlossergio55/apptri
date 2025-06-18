using Syncfusion.Blazor;

namespace Server.Shared
{
    public class SyncfusionLocalizer : ISyncfusionStringLocalizer
    {        
        public string GetText(string key)
        {
            return this.ResourceManager.GetString(key);
        }
        public System.Resources.ResourceManager ResourceManager
        {
            get
            {                
                return  Server.Resources.SfResources.ResourceManager;
            }
        }
    }
}
