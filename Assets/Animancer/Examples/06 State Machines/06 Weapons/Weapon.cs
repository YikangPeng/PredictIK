// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using UnityEngine;

namespace Animancer.Examples.StateMachines.Weapons
{
    /// <summary>
    /// Holds various animations relating to the use of a weapon. In a real game, this class might have other details
    /// like damage, damage type, weapon category, etc. It could also inherit from a base Item class for things like
    /// weight, cost, and description.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/weapons">Weapons</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.Weapons/Weapon
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Weapons - Weapon")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(Weapons) + "/" + nameof(Weapon))]
    public sealed class Weapon : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private ClipTransition[] _AttackAnimations;
        public ClipTransition[] AttackAnimations => _AttackAnimations;

        /************************************************************************************************************************/

        [SerializeField]
        private ClipTransition _EquipAnimation;
        public ClipTransition EquipAnimation => _EquipAnimation;

        [SerializeField]
        private ClipTransition _UnequipAnimation;
        public ClipTransition UnequipAnimation => _UnequipAnimation;

        /************************************************************************************************************************/
    }
}
