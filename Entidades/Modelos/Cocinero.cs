
using Entidades.Files;
using Entidades.Interfaces;
using System.ComponentModel.Design;
using System.Threading;

namespace Entidades.Modelos
{

    public delegate void DelegadoNuevoIngreso(IComestible menu);
    public delegate void DelegadoDemoraAtencion(double demora);


    public class Cocinero<T> where T : IComestible, new()
    {
        public event DelegadoNuevoIngreso OnIngreso;
        public event DelegadoDemoraAtencion OnDemora;

        private int cantPedidosFinalizados;
        private string nombre;
        private double demoraPreparacionTotal;
        private CancellationTokenSource cancellation;
        private T menu;
        private Task tarea;



        public Cocinero(string nombre)
        {
            this.nombre = nombre;
        }

        //No hacer nada
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
                    this.IniciarIngreso();
                }
                else
                {
                    this.cancellation.Cancel();
                }
            }
        }

        //no hacer nada
        public double TiempoMedioDePreparacion { get => this.cantPedidosFinalizados == 0 ? 0 : this.demoraPreparacionTotal / this.cantPedidosFinalizados; }
        public string Nombre { get => nombre; }
        public int CantPedidosFinalizados { get => cantPedidosFinalizados; }



        private void IniciarIngreso()
        {
            tarea = Task.Run(async () =>
            {
                try
                {
                    while (!cancellation.IsCancellationRequested)
                    {
                        NotificarNuevoIngreso();
                        EsperarProximoIngreso();
                        cantPedidosFinalizados++;
                        DataBaseManager.GuardarTicket(nombre, menu);
                    }
                }
                catch (Exception ex)
                {
                    FileManager.Guardar($"Error: {ex.Message}", "logs.txt", true);

                }
            }, cancellation.Token);
        }
        private void NotificarNuevoIngreso()
        {
            if (OnIngreso != null)
            {
                try
                {
                    this.menu = new T();

                    this.menu.IniciarPreparacion();

                    this.OnIngreso.Invoke(menu);
                }
                catch (Exception ex)
                {
                    FileManager.Guardar($"Error: {ex.Message}", "logs.txt", true);

                }
            }
        }
        private void EsperarProximoIngreso()
        {
            if (OnDemora != null)
            {
                int tiempoEspera = 0;

                try
                {
                    while (true)
                    {
                        OnDemora.Invoke(tiempoEspera);

                        if (menu.Estado || cancellation.IsCancellationRequested)
                        {
                            break;
                        }

                        // Esperar 1 segundo antes de continuar
                        Thread.Sleep(1000);

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
        }
    }
 }
