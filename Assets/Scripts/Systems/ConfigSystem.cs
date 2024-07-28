using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine.AddressableAssets;

namespace Systems
{
    public class ConfigSystem : AbstractSystem
    {
        private CharacterConfig _characterConfig;

        protected override async void OnInit()
        {
            await LoadConfig();
        }

        private async UniTask LoadConfig()
        {
            var operationHandle = Addressables.LoadAssetAsync<CharacterConfig>("character_config");
            _characterConfig = await operationHandle;
        }

        public async UniTask<CharacterConfig> GetCharacterConfig()
        {
            if (!_characterConfig)
            {
                await LoadConfig();
            }

            return _characterConfig;
        }
    }
}