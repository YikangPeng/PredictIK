// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer.FSM;
using UnityEngine;

namespace Animancer.Examples.StateMachines.InterruptManagement
{
    /// <summary>
    /// A state for a <see cref="Character"/> which plays an animation and uses a <see cref="Priority"/>
    /// enum to determine which other states can interrupt it.
    /// </summary>
    /// <example><see href="https://kybernetik.com.au/animancer/docs/examples/fsm/interrupts">Interrupt Management</see></example>
    /// https://kybernetik.com.au/animancer/api/Animancer.Examples.StateMachines.InterruptManagement/CharacterState
    /// 
    [AddComponentMenu(Strings.ExamplesMenuPrefix + "Interrupt Management - Character State")]
    [HelpURL(Strings.DocsURLs.ExampleAPIDocumentation + nameof(StateMachines) + "." + nameof(InterruptManagement) + "/" + nameof(CharacterState))]
    public sealed class CharacterState : Characters.CharacterState
    {
        /************************************************************************************************************************/
        // Note that this class inherits from Characters.CharacterState from the previous example.
        /************************************************************************************************************************/

        /// <summary>Levels of importance.</summary>
        public enum Priority
        {
            Low,// Could specify "Low = 0," if we want to be explicit.
            Medium,// Medium = 1,
            High,// High = 2,
        }

        /************************************************************************************************************************/

        [SerializeField] private Priority _Priority;

        /************************************************************************************************************************/

        /// <summary>
        /// Only allows a new state to be entered if it has equal or higher <see cref="Priority"/> to this state.
        /// </summary>
        public override bool CanExitState
        {
            get
            {
                var nextState = (CharacterState)StateChange<Characters.CharacterState>.NextState;
                return nextState._Priority >= _Priority;
            }
        }

        // Access the StateChange directly using its static property.
        // StateChange<CharacterState>.NextState

        // Avoid needing to specify the <CharacterState> parameter by accessing it via the StateMachine.
        // _Character.StateMachine.NextState

        // Or even with an IState extension method.
        // this.GetNextState()

        /************************************************************************************************************************/
    }
}
