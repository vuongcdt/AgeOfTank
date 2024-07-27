using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
            // AsyncOperationHandle<CharacterConfig> configAsyncOperationHandle =
            //     Addressables.LoadAssetAsync<CharacterConfig>("character_config");
            // _characterConfig = await configAsyncOperationHandle;

            _characterConfig = await Addressables.LoadAssetAsync<CharacterConfig>("character_config");
        }

        public async UniTask<CharacterConfig> GetCharacterConfig()
        {
            if (!_characterConfig)
            {
                await LoadConfig();
            }

            return _characterConfig;
        }

        public float GetCharacterHealConfig(ENUMS.CharacterTypeClass typeClass)
        {
            return _characterConfig.unitConfigs[(int)typeClass].health;
        }
    }
}