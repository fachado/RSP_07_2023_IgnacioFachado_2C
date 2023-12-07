
using Entidades.Files;
using Entidades.Interfaces;
using System.ComponentModel.Design;
using System.Threading;
using Entidades.Modelos;
namespace Entidades.Modelos
{

    public class Cocinero<T> where T : IComestible, new()
    {
        public delegate void DelegadoPedidoEnCurso(T pedido);
        public delegate void DelegadoDemoraAtencion(double demora);

        public event DelegadoPedidoEnCurso OnPedido;
        public event DelegadoDemoraAtencion OnDemora;

        private CancellationTokenSource cancellation;
        private int cantPedidosFinalizados;
        private double demoraPreparacionTotal;
        private Mozo<T> mozo;
        private string nombre;
        private T pedidoEnPreparacion;
        private Queue<T> pedidos;
        private Task tarea;

        public Cocinero(string nombre)
        {
            this.nombre = nombre;
            this.mozo = new Mozo<T>();
            this.pedidos = new Queue<T>();
            this.mozo.OnPedido += TomarNuevoPedido;
        }

        public bool HabilitarCocina
        {
            get
            {
                return this.tarea is not null && (this.tarea.Status == TaskStatus.Running ||
                                                  this.tarea.Status == TaskStatus.WaitingToRun ||
                                                  this.tarea.Status == TaskStatus.WaitingForActivation);
            }
            set
            {
                if (value && !this.HabilitarCocina)
                {
                    this.cancellation = new CancellationTokenSource();
                    this.mozo.EmpezarATrabajar = true;
                    this.EmpezarACocinar();
                }
                else
                {
                    this.cancellation.Cancel();
                    this.mozo.EmpezarATrabajar = false;
                }
            }
        }

        public double TiempoMedioDePreparacion
        {
            get => this.cantPedidosFinalizados == 0 ? 0 : this.demoraPreparacionTotal / this.cantPedidosFinalizados;
        }

        public string Nombre => nombre;
        public int CantPedidosFinalizados => cantPedidosFinalizados;

        public Queue<T> Pedidos => new Queue<T>(pedidos);

        private void EmpezarACocinar()
        {
            tarea = Task.Run(async () =>
            {
                try
                {
                    while (!cancellation.IsCancellationRequested)
                    {
                        if (pedidos.Count > 0)
                        {
                            pedidoEnPreparacion = pedidos.Dequeue();
                            OnPedido.Invoke(pedidoEnPreparacion);
                            await EsperarProximoIngreso();
                            cantPedidosFinalizados++;
                            DataBaseManager.GuardarTicket(nombre, pedidoEnPreparacion);
                        }
                    }
                }
                catch (Exception ex)
                {
                    FileManager.Guardar($"Error: {ex.Message}", "logs.txt", true);
                }
            }, cancellation.Token);
        }

        private async Task EsperarProximoIngreso()
        {
            int tiempoEspera = 0;

            try
            {
                while (true)
                {
                    OnDemora?.Invoke(tiempoEspera);

                    if (pedidoEnPreparacion.Estado || cancellation.IsCancellationRequested)
                    {
                        break;
                    }

                    // Esperar 1 segundo antes de continuar
                    await Task.Delay(1000);

                    tiempoEspera++;
                }
            }
            catch (Exception ex)
            {
                FileManager.Guardar($"Error: {ex.Message}", "logs.txt", true);
            }
            finally
            {
                demoraPreparacionTotal += tiempoEspera;
            }
        }

        private void TomarNuevoPedido(T pedido)
        {
            if (OnPedido != null)
            {
                pedidos.Enqueue(pedido);
            }
        }
    }
}
