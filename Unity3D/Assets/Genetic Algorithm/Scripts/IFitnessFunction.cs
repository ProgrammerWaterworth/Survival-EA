using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFitnessFunction
{


    /// <summary>
    /// The Fitness of an individual. 
    /// A greater fitness indicates an individual which is better at acheiving their goal.
    /// </summary>
    /// <returns>A value to indicate the success of the individual.</returns>
    float GetFitness();

    /// <summary>
    /// Determines whether the individual is still in a simulation.
    /// </summary>
    /// <returns>True: if simulation is over and fitness value has been produced. False: Simulation still running.</returns>
    bool IsEvalutionComplete();


}
