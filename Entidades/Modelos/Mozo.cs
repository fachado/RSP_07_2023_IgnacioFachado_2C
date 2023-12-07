using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades.Files;
using Entidades.Interfaces;
using System.ComponentModel.Design;
using System.Threading;

namespace Entidades.Modelos
{

    public class Mozo<T> where T : IComestible, new()
    {
        private CancellationTokenSource cancellation;
        private T menu;
        private Task tarea;

        public delegate void DelegadoNuevoPedido(T menu);

        public event DelegadoNuevoPedido OnPedido;

        public bool EmpezarATrabajar
        {
            get
            {
                return tarea != null && (tarea.Status == TaskStatus.Running || tarea.Status == TaskStatus.WaitingToRun || tarea.Status == TaskStatus.WaitingForActivation);
            }
            set
            {
                if (value)
                {
                    if (tarea == null || tarea.Status != TaskStatus.Running)
                    {
                        cancellation = new CancellationTokenSource();
                        TomarPedidos();
                    }
                }
                else
                {
                    if (tarea != null && (tarea.Status == TaskStatus.Running || tarea.Status == TaskStatus.WaitingToRun || tarea.Status == TaskStatus.WaitingForActivation))
                    {
                        cancellation?.Cancel();
                    }
                }
            }
        }

        private void TomarPedidos()
        {
            tarea = Task.Run(() =>
            {
                while (!cancellation.Token.IsCancellationRequested)
                {
                    NotificarNuevoPedido();
                    Thread.Sleep(5000); // Espera 5 segundos antes de tomar otro pedido
                }
            }, cancellation.Token);
        }

        private void NotificarNuevoPedido()
        {
            if (OnPedido != null)
            {
                T nuevoMenu = new T(); 

                nuevoMenu.IniciarPreparacion();
 
                OnPedido.Invoke(nuevoMenu); 
            }
        }
    }

}

