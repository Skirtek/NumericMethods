using System.Collections.Generic;
using NumericMethods.Models;

namespace NumericMethods.Interfaces
{
    public interface IEquation
    {
        /// <summary>
        /// Solves equation with two variables
        /// </summary>
        /// <param name="equationsList">List of equations given by user</param>
        /// <returns>Solution</returns>
        string TwoVariablesEquation(List<Equation> equationsList);

        /// <summary>
        /// Solves equation with three variables
        /// </summary>
        /// <param name="equationsList">List of equations given by user</param>
        /// <returns>Solution</returns>
        string ThreeVariablesEquation(List<Equation> equationsList);

        /// <summary>
        /// Solves equation with four variables
        /// </summary>
        /// <param name="equationsList">List of equations given by user</param>
        /// <returns>Solution</returns>
        string FourVariablesEquation(List<Equation> equationsList);
    }
}