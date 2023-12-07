using Entidades.Enumerados;

public static class IngredienteExtensions
{
    public static double CalcularCostoIngrediente(this List<EIngrediente> ingredientes, int costoInicial)
    {
       
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
                return 5.0; 
            case EIngrediente.PANCETA:
                return 8.0; 
            case EIngrediente.ADHERESO:
                return 3.0; 
            case EIngrediente.HUEVO:
                return 6.0; 
            case EIngrediente.JAMON:
                return 4.0; 
            default:
                return 0.0; 
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