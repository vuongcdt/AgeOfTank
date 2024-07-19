using QFramework;

namespace Interfaces
{
    public interface IGameStorage:IUtility
    {
        void SaveInt(string key, int value);
        int LoadInt(string key, int defaultValue = 0);
        
        void SaveFloat(string key, float value);
        float LoadFloat(string key, float defaultValue = 0.5f);
        
        void SaveString(string key, string value);
        string LoadString(string key);
    }
}