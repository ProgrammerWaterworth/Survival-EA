using System.Collections.Generic;

/// <summary>
/// Any agent that wants to use GOAP must implement this interface. 
/// Provides information to the GOAP planner to create action plans.
/// Interface also reports Success/Failure.
/// </summary>
public interface IGoap
{
	/// <summary>
	/// The starting state of the Agent and the world.
	/// Supply what states are needed for actions to run.
	/// </summary>
	/// <returns>Starting state.</returns>
	HashSet<KeyValuePair<string, object>> GetWorldState();

	/// <summary>
	/// Create a Goal state so the Planner can calculate which actions best lead to the goal state.
	/// </summary>
	/// <returns>Goal State.</returns>
	HashSet<KeyValuePair<string, object>> CreateGoalState();

	/// <summary>
	/// No sequence of actions could be found for the supplied goal.
	/// You will need to try another goal or try again once world state has changed.
	/// </summary>
	/// <param name="_failedGoal"></param>
	void PlanFailed(HashSet<KeyValuePair<string, object>> _failedGoal);

	/// <summary>
	/// A plan was found for the supplied goal. 
	/// </summary>
	/// <param name="_goalState">Goal to plan for.</param>
	/// <param name="_actions">Ordered queue of actions the GOAP Agent will perform</param>
	void PlanFound(HashSet<KeyValuePair<string, object>> _goalState, Queue<GoapAction> _actions);

	/// <summary>
	/// Goal reached and all actions completed.
	/// </summary>
	void ActionsFinished();

	/// <summary>
	/// An action caused the plan to abort. That action is returned.
	/// </summary>
	/// <param name="_aborter">Action that aborted the plan.</param>
	void PlanAborted(GoapAction _aborter);

	/// <summary>
	/// An Update Function. Move agent in range of next action to perform.
	/// </summary>
	/// <param name="_nextAction">Next action to perform once in range.</param>
	/// <returns>Return true if can perform action. False if not in range.</returns>
	bool MoveAgent(GoapAction _nextAction);

	/// <summary>
	/// Checks to see if there are any goals to try plan for.
	/// </summary>
	/// <returns></returns>
	bool HasGoalsLeft();
}

