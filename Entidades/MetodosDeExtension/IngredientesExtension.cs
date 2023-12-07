using Entidades.Enumerados;

public static class IngredienteExtensions
{
    public static double CalcularCostoIngrediente(this List<EIngrediente> ingredientes, int costoInicial)
    {
        // Calcula el costo total de los ingredientes aplicando incrementos porcentuales
        double costoTotal = costoInicial;

        foreach (var ingrediente in ingredientes)
        {
            double porcentajeIncremento = ObtenerPorcentajeIncremento(ingrediente);
            costoTotal += costoTotal * (porcentajeIncremento / 100);
        }

        return costoTotal;
    }

    private static double ObtenerPorcentajeIncremento(EIngrediente ingrediente)
    {

      
        switch (ingrediente)
        {
            case EIngrediente.QUESO:
                return 5.0; // Incremento del 5% para el queso
            case EIngrediente.PANCETA:
                return 8.0; // Incremento del 8% para la panceta
            case EIngrediente.ADHERESO:
                return 3.0; // Incremento del 3% para el aderezo
            case EIngrediente.HUEVO:
                return 6.0; // Incremento del 6% para el huevo
            case EIngrediente.JAMON:
                return 4.0; // Incremento del 4% para el jamón
            default:
                return 0.0; // Sin incremento para otros ingredientes
        }
    }
}

public static class RandomExtensions
{
    public static List<EIngrediente> GenerarIngredientesAleatorios(this Random rand)
    {
        List<EIngrediente> ingredientesDisponibles = Enum.GetValues(typeof(EIngrediente)).Cast<EIngrediente>().ToList();
        int cantidadIngredientes = rand.Next(1, ingredientesDisponibles.Count + 1);
        return ingredientesDisponibles.OrderBy(i => rand.Next()).Take(cantidadIngredientes).ToList();
    }
}