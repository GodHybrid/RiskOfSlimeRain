using RiskOfSlimeRain.Helpers;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.NPCs
{
	/// <summary>
	/// Defines a transition starting from a state, triggered via command
	/// </summary>
	/// <typeparam name="TState">Transition originates from this state</typeparam>
	/// <typeparam name="TCommand">Transition is triggered by this command</typeparam>
	public class StateTransition<TState, TCommand>
		where TState : Enum
		where TCommand : Enum
	{
		public readonly TState CurrentState;
		public readonly TCommand Command;

		/// <summary>
		/// Defines a transition starting from a state, triggered via command
		/// </summary>
		/// <param name="currentState">Transition originates from this state</param>
		/// <param name="command">Transition is triggered by this command</param>
		public StateTransition(TState currentState, TCommand command)
		{
			CurrentState = currentState;
			Command = command;
		}

		public override int GetHashCode()
		{
			return 17 + 31 * (CurrentState.GetHashCode() + Command.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			return obj is StateTransition<TState, TCommand> other && CurrentState.Equals(other.CurrentState) && Command.Equals(other.Command);
		}

		public override string ToString()
		{
			return $"if {Command} while {CurrentState}:";
		}
	}

	public abstract class FSM<TState, TCommand>
		where TState : Enum
		where TCommand : Enum
	{
		/// <summary>
		/// Mapped state transitions to their new state
		/// </summary>
		public Dictionary<StateTransition<TState, TCommand>, TState> Transitions { get; protected set; }

		public TState CurrentState { get; protected set; }

		private bool valid = false;

		protected FSM()
		{

		}

		/// <summary>
		/// Go to next state based on command. Stays on current state if no transition found
		/// </summary>
		public TState GetNext(StateTransition<TState, TCommand> transition)
		{
			TState nextState = CurrentState;
			if (Transitions.ContainsKey(transition))
			{
				nextState = Transitions[transition];
			}
			//else
			//{
			//	Debug, remove in release
			//	GeneralHelper.Print("Invalid transition from " + CurrentState + " via " + transition.Command);
			//}
			return nextState;
		}

		/// <summary>
		/// Go to next state based on command. Stays on current state if no transition found
		/// </summary>
		public TState MoveNext(StateTransition<TState, TCommand> transition)
		{
			CheckValidity();
			var nextState = GetNext(transition);
			CurrentState = nextState;
			return CurrentState;
		}

		/// <summary>
		/// Go to next state based on command. Stays on current state if no transition found
		/// </summary>
		public TState MoveNext(TCommand command)
		{
			var transition = new StateTransition<TState, TCommand>(CurrentState, command);
			return MoveNext(transition);
		}

		private void CheckValidity()
		{
			if (valid) return;

			bool exists;
			var states = Enum.GetValues(typeof(TState));

			foreach (var state in states)
			{
				exists = false;
				foreach (var transition in Transitions)
				{
					if (transition.Key.CurrentState.Equals(state))
					{
						exists = true;
						break;
					}
				}
				if (!exists)
				{
					throw new Exception(state + " doesn't exist in the transition table");
				}
			}

			valid = true;
		}

		public override string ToString()
		{
			return $"{CurrentState}";
		}
	}

	public abstract class NPCFSM<TNPC, TState, TCommand> : FSM<TState, TCommand>
		where TNPC : ModNPC
		where TState : Enum
		where TCommand : Enum
	{
		/// <summary>
		/// The modded NPC this FSM is tied to
		/// </summary>
		public TNPC Me { get; private set; }

		public NPCFSM(TNPC modNPC) : base()
		{
			Me = modNPC;
		}

		/// <summary>
		/// Override to set conditions to when states are switched. Operate on CurrentState
		/// </summary>
		public abstract void UpdateState();

		/// <summary>
		/// Operate based on CurrentState
		/// </summary>
		public abstract void ExecuteCurrentState();
	}
}
