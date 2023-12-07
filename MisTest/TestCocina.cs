// using Entidades.Exceptions; Da error
using Entidades.Files;
using Entidades.Interfaces;
using Entidades.Modelos;

namespace MisTest
{
    [TestClass]
    public class TestCocina
    {
        [TestMethod]
        [ExpectedException(typeof(FileManagerException))]
        public void AlGuardarUnArchivo_ConNombreInvalido_TengoUnaExcepcion()
        {
            //arrange
            FileManager.Guardar("shalom", "\"sarakatumba #!.T#$|.\"", true);

            //act

            //assert

        }

        [TestMethod]

        public void AlInstanciarUnCocinero_SeEspera_PedidosCero()
        {
            //arrange
            Cocinero<Hamburguesa> PruebaPedidos;


            //act
            PruebaPedidos = new Cocinero<Hamburguesa>("Nachito");
            //assert
            Assert.AreEqual(PruebaPedidos.CantPedidosFinalizados, 0);
        }
    }
}