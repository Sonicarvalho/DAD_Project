using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

delegate void ThrWork();

namespace pacman.ChatResources
{
    class ThrPool
    {
        private CircularBuffer<ThrWork> buf;
        private Thread[] pool;
        public ThrPool(int thrNum, int bufSize)
        {
            buf = new CircularBuffer<ThrWork>(bufSize);
            pool = new Thread[thrNum];
            for (int i = 0; i < thrNum; i++)
            {
                pool[i] = new Thread(new ThreadStart(consomeExec));
                pool[i].Start();
            }
        }

        public void AssyncInvoke(ThrWork action)
        {
            buf.Produce(action);

            Console.WriteLine("Submitted action");
        }

        public void consomeExec()
        {
            while (true)
            {
                ThrWork tw = buf.Consume();
                tw();
            }
        }
    }


    class A
    {
        private int _id;

        public A(int id)
        {
            _id = id;
        }

        public void DoWorkA()
        {
            Console.WriteLine("A-{0}", _id);
        }
    }


    class B
    {
        private int _id;

        public B(int id)
        {
            _id = id;
        }

        public void DoWorkB()
        {
            Console.WriteLine("B-{0}", _id);
        }
    }


    /*class Test
    {
        public static void Main()
        {
            ThrPool tpool = new ThrPool(5, 10);
            ThrWork work = null;
            for (int i = 0; i < 5; i++)
            {
                A a = new A(i);
                tpool.AssyncInvoke(new ThrWork(a.DoWorkA));
                B b = new B(i);
                tpool.AssyncInvoke(new ThrWork(b.DoWorkB));
            }
            Console.ReadLine();
        }
    }*/
}
