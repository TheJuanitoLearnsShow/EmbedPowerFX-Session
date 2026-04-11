using Microsoft.PowerFx;
using Microsoft.PowerFx.Types;

namespace BlazorAppSample2
{
    public class SamplePowerFx
    {
        public static decimal GetTestValue()
        {
            var engine = new RecalcEngine();

            engine.UpdateVariable("EmailTxt", "jj@gamil.com");

            // notice the escaped quotes as we are passing a PowerFX string literal. Our second parameter needs to be 
            //  a valid PowerFX expression.
            engine.SetFormula("MyName", "\"Juanito\"", OnFormulaUpdate);

            // engine.SetFormula("MyName", "\"Juanito\"", OnFormulaUpdate); // throws error because MyName is already defined (System.InvalidOperationException: MyName is already defined)

            engine.SetFormula("Greeting", "\"Hello, \" & MyName & \"!\"", OnFormulaUpdate);
            engine.SetFormula("IsEmail",
                """
            If(
                Find(
                   "@",
                   EmailTxt
                ),
                true,
                false
            )
            """, OnFormulaUpdate);

            var greetingResult = engine.Eval("Greeting");
            var isEmailResult = engine.Eval("IsEmail");

            var isEmailResult2 = engine.Eval("IsEmail");

            // 2nd Example
            engine.UpdateVariable("Velocity", 20);
            engine.SetFormula("Mass", "2", OnFormulaUpdate);
            engine.SetFormula("KineticEnergy", "(1/2) * Mass * Velocity * Velocity", OnFormulaUpdate);
            var energyResult = engine.GetValue("KineticEnergy");
            return (energyResult as DecimalValue).Value;
        }

        private static void OnFormulaUpdate(string arg1, FormulaValue value)
        {
        }
    }
}
