using Interfaces;
using QFramework;
using UnityEngine;

namespace Systems
{
    public class GameSystem:AbstractSystem,IGameSystem
    {
        protected override void OnInit()
        {
            this.GetModel<IGamePlayUIModel>()
                .Count
                .Register(newCount =>
                {
                    if (newCount == 2)
                    {
                        Debug.Log(1);
                    }
                    else if (newCount == 4)
                    {
                        Debug.Log(2);
                    }
                    else if (newCount == -2)
                    {
                        Debug.Log(3);
                    }
                });
        }
    }
}