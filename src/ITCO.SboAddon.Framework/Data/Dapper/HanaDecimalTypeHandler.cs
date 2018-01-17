namespace ITCO.SboAddon.Framework.Data.Dapper
{
    using System.Data;

    using global::Dapper;

    using Sap.Data.Hana;

    /// <summary>
    /// Type-handler for HanaDecimal.
    /// </summary>
    public class HanaDecimalTypeHandler : SqlMapper.TypeHandler<decimal>
    {
        /// <summary>
        /// Default handler instance
        /// </summary>
        public static readonly HanaDecimalTypeHandler Default = new HanaDecimalTypeHandler();

        /// <summary>
        /// Parse a database value back to a typed value.
        /// </summary>
        /// <param name="value">The value from the database.</param>
        /// <returns>The typed value.</returns>
        public override decimal Parse(object value)
        {
            if (value is HanaDecimal)
            {
                return ((HanaDecimal)value).ToDecimal();
            }

            return decimal.Parse(value.ToString());
        }

        /// <summary>
        /// Assign the value of a parameter before a command executes.
        /// </summary>
        /// <param name="parameter">The parameter to configure.</param>
        /// <param name="value">Parameter value.</param>
        public override void SetValue(IDbDataParameter parameter, decimal value)
        {
            parameter.Value = value;
        }
    }
}
