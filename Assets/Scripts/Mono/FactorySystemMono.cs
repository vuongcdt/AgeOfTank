using System;
using UnityEngine;

namespace Mono
{
    public class FactorySystemMono : MonoBehaviour
    {
        public Action OnUpdate;

        private void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}