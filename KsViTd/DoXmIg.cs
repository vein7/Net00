using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks.Dataflow;

namespace KsViTd {
    class DoXmIg {

        public DoXmIg Test() {
            TestInvoke2();

            System.Xml.Linq.XNamespace ns1 = "";



            return this;
        }

        #region 委托

        /// <summary>
        /// 异步委托
        /// </summary>
        /// <returns></returns>
        public DoXmIg TestInvoke1() {
            Func<int, int, int> fun = (a, sleepMs) => {
                System.Threading.Thread.Sleep(sleepMs);
                return ++a;
            };

            var asyncR = fun.BeginInvoke(2, 5000
                , (o) => Console.WriteLine(o.AsyncState)
                , "a");

            // asyncR.IsCompleted 线程是否执行完成。
            //while(false == asyncR.IsCompleted) {
            //    Console.Write(".");
            //    System.Threading.Thread.Sleep(200);
            //}

            var funResult = fun.EndInvoke(asyncR);      // EndInvoke 会一直等待委托执行完成。

            return this;
        }


        /// <summary>
        /// 等待句柄
        /// </summary>
        /// <returns></returns>
        public DoXmIg TestInvoke2() {
            Func<int, int, int> fun = (a, sleepMs) => {
                var count = sleepMs / 200;
                for (var i = 0; i < count; ++i) {
                    Console.Write("+");
                    System.Threading.Thread.Sleep(200);
                }
                System.Threading.Thread.Sleep(sleepMs % 200);

                return ++a;
            };

            var asyncR = fun.BeginInvoke(2, 5000
                , (o) => Console.WriteLine(o.AsyncState)
                , "a");

            while (true) {
                Console.Write("_");
                if (asyncR.AsyncWaitHandle.WaitOne(1000, true)) {
                    Console.WriteLine("Ok");
                    break;
                }
                Console.Write(".");
                System.Threading.Thread.Sleep(200);
            }

            var funResult = fun.EndInvoke(asyncR);      // EndInvoke 会一直等待委托执行完成。

            return this;
        }

        /// <summary>
        /// 异步回调
        /// </summary>
        /// <returns></returns>
        public DoXmIg TestInvoke3() {
            Func<int, int, int> fun = (a, sleepMs) => {
                System.Threading.Thread.Sleep(sleepMs);
                return ++a;
            };

            // 倒数第二个参数, 可以传递一个回调函数, 这个函数是委托线程中调用的, 而不是在主线程中.
            // 最后一个参数, 可以传递任何对象, 可以传递委托本身, 然后使用 EndInvoke 获取返回结果
            var asyncR = fun.BeginInvoke(2, 2000
                , (ar) => {
                    Console.WriteLine(
                        (ar.AsyncState as Func<int, int, int>).EndInvoke(ar));
                }
                , fun);


            System.Diagnostics.Trace.Assert(false);
            return this;
        }

        #endregion

        #region Thread
        // 只要有一个前台线程在运行, 应用程序的进程就在运行.
        // 如果有多个前台线程在运行, 而 Main() 方法结束了, 程序的进程仍然是激活的, 直到所有前台进程执行完成为止.
        // 默认情况下, 用 Thread 类创建的线程是前台线程, 线程池中的线程是后台线程.


        public DoXmIg Thread1() {
            new Thread(
                obj => Console.WriteLine(obj))
            .Start("start");


            Console.WriteLine("---");
            return this;
        }

        public DoXmIg Thread2() {
            var t = new Thread(obj => {
                try {
                    Console.WriteLine("..");
                    Thread.Sleep(3000);
                    Console.WriteLine(obj);
                } catch (Exception e) {
                    throw e;
                }
            });

            t.Start(2222);
            Thread.Sleep(1000);
            t.Abort();


            Console.WriteLine("---");
            return this;
        }

        public DoXmIg Thread3() {
            return this;
        }

        #endregion


        // 线程池中的所有线程都是后台线程,
        // 不能给线程池中的线程设置优先级或名称
        // 入池的线程应该是短时间工作的, 而不是那种一直运行的线程.
        public DoXmIg ThreadPool1() {
            ThreadPool.GetMaxThreads(out var workerThreads, out var completionPortThreads);
            Console.WriteLine($"最大工作线程:{ workerThreads }, 最大IO线程:{ workerThreads }");


            WaitCallback call = (obj) => {
                Thread.Sleep(1000);
                Console.WriteLine(obj + "," + Thread.CurrentThread.ManagedThreadId);
            };

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            Console.WriteLine("-----------");
            for (var i = 0; i < 100; ++i) {
                ThreadPool.QueueUserWorkItem(call, "A");     //加入队列的时候就直接运行了.
            }
            ThreadPool.QueueUserWorkItem(
                (obj) => {
                    var watch = (obj as System.Diagnostics.Stopwatch);
                    watch.Stop();
                    Console.WriteLine($"当前耗时:" + watch.Elapsed);
                }
                , stopwatch);

            //AutoResetEvent
            //Interlocked.Decrement()

            return this;
        }

        public DoXmIg ThreadPool2() {
            //ExecutionContext
            // 将一些数据放到当前线程的“执行上下文”
            System.Runtime.Remoting.Messaging.CallContext.LogicalSetData("Name", "AAA");

            ThreadPool.QueueUserWorkItem(async state => {
                await Task.Delay(1000 * 2);
                Console.WriteLine("Name=" + System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("Name"));
            });

            // 阻止当前线程的“执行上下文”流动
            ExecutionContext.SuppressFlow();

            ThreadPool.QueueUserWorkItem(
                state => Console.WriteLine("Name=" + System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("Name")));

            ExecutionContext.RestoreFlow();
            Console.WriteLine("RestoreFlow");

            return this;
        }

        #region Task


        public DoXmIg Task1() {
            Action<object> call = (obj) => {
                Console.WriteLine($"TaskId:{Task.CurrentId}, {obj}");
            };

            // 
            new TaskFactory().StartNew(call, "new TaskFactory");

            //new Task()    TaskStatus.Created 
            //.Start()      开始执行
            new Task(call, "new Task").Start();

            // 
            Task.Factory.StartNew(call, "Task.Factory.StartNew");

            // TaskCreationOptions.LongRunning 通知任务调度器, 该任务需要较长时间, 这样调度器更可能会使用新线程
            Task.Factory.StartNew(
                () => {
                    for (var i = 0; i < 10; ++i) {
                        Thread.Sleep(200);
                        Console.Write(".");
                    }
                    Console.WriteLine(";");
                }
                , TaskCreationOptions.LongRunning);

            Task.Run(() => Console.WriteLine("0000000"));

            return this;
        }

        /// <summary>
        /// 连续任务
        /// </summary>
        /// <returns></returns>
        public DoXmIg Task2() {
            Func<object, string> fun1 = (a) => {
                Console.WriteLine($".....fun1-start, {DateTime.Now.ToLongTimeString()}, ");
                Thread.Sleep(1200);
                Console.WriteLine($"...........fun1, {DateTime.Now.ToLongTimeString()}, " + a);
                return $"[fun1 :{a}]";
            };

            Func<Task<string>, object, string> fun2 = (t, s) => {
                Console.WriteLine($".....fun2-start, {DateTime.Now.ToLongTimeString()}, ");
                Thread.Sleep(3000);
                Console.WriteLine($"...........fun2, {DateTime.Now.ToLongTimeString()}, " + s);
                return $"{t.Result},[fun2 :{s}]";
            };
            Func<Task<string>, object, string> fun3 = (t, s) => {
                Thread.Sleep(5100);
                Console.WriteLine($"...........fun3, {DateTime.Now.ToLongTimeString()}, " + s);
                return $"{t.Result},[fun3 :{s}]";
            };

            // 先执行 fun1, 结束后执行两个任务 fun2,fun3
            // fun3 执行完后, 再执行附加任务
            var task1 = new Task<string>(fun1, 1000);
            var task2 = task1.ContinueWith(fun2, "sssss");
            task1.ContinueWith(fun3, "aaaaaaaaaa")
                .ContinueWith((t) => Console.WriteLine(".........fun3 之后的任务"), TaskContinuationOptions.NotOnFaulted);

            task1.Start();// 0.2s 左右 task1 执行完毕, 然后异步执行 fun2, fun3, 当前线程执行下面的代码, 
            //task2.Wait();     可以手动等待线程执行完毕

            // 调用 task2.Result 会阻塞当前线程, 等待 task2 的结果
            Console.WriteLine("\n" + "Result:" + task2.Result);
            return this;
        }

        /// <summary>
        /// 连续任务
        /// </summary>
        /// <returns></returns>
        public DoXmIg Task3() {
            Action child = () => {
                Console.WriteLine("Id:" + Task.CurrentId);
                for (var i = 0; i < 20; ++i) {
                    Thread.Sleep(200);
                    Console.Write(",");
                }
            };

            Action parent = () => {
                Console.WriteLine("Id:" + Task.CurrentId);

                Task.Factory.StartNew(child);

                for (int i = 0; i < 20; ++i) {
                    Thread.Sleep(300);
                    Console.Write("-");
                }
            };

            Task.Factory.StartNew(parent);


            return this;
        }

        public DoXmIg TaskEx() {
            Func<CancellationToken, int, int> fnSum = (ct, n) => {
                int sum = 0;
                for (; n > 0; n--) {
                    // 抛出 OperationCanceledException 异常，表示 Task 没有一直执行结束
                    ct.ThrowIfCancellationRequested();
                    checked { sum += n; }
                }
                return sum;
            };

            var cts = new CancellationTokenSource();
            var t1 = Task.Run(() => fnSum(cts.Token, 1_000_000_000));
            //cts.Cancel();     // 如果取消之前 Task 没有执行 Start，会直接取消，不会执行
            cts.CancelAfter(100);

            try {
                // 可能会抛出 AggregateException 异常，可能是 OperationCanceledException 或 OverflowException
                Console.WriteLine($"sum = {t1.Result}");
            } catch (AggregateException aEx) {
                try {
                    // 如果还存在未处理的异常会继续抛出新的 AggregateException
                    aEx.Handle(e => e is OperationCanceledException);
                    Console.WriteLine("canceled");
                } catch (AggregateException aEx2) {
                    aEx2.Handle(e => e is OverflowException);
                    Console.WriteLine("run fnSum --OverflowException");
                }
            }

            return this;
        }

        public static void Task_ContinuationOptions() {
            var task = new Task(() => {
                throw new InvalidOperationException();
            });
            task.ContinueWith((t) => Console.WriteLine("出了异常，这算不算完成"));
            task.Start();

            var taskOk = new Task(() => {
                Task.Delay(10).Wait();
            });
            taskOk.ContinueWith((t) => Console.WriteLine("OnlyOnFaulted"), TaskContinuationOptions.OnlyOnFaulted);
            taskOk.Start();

            var cts = new CancellationTokenSource();
            var taskCancel = new Task(() => {
                Task.Delay(10).Wait();
            }, cts.Token);
            taskCancel.ContinueWith((t) => Console.WriteLine("OnlyOnCanceled"), TaskContinuationOptions.OnlyOnCanceled);
            taskCancel.ContinueWith((t) => Console.WriteLine("NotOnCanceled"), TaskContinuationOptions.NotOnCanceled);
            taskCancel.Start();
            cts.Cancel();

            var taskCancel2 = new Task(() => {
                Console.WriteLine("taskCancel2");
                throw new TaskCanceledException("taskCancel2");     // taskCancel2.Status 不是 Canceled
            });
            taskCancel2.Start();
            Console.WriteLine($"taskCancel2.Status: {taskCancel2.Status}");

        }

        public static void TaskCancel() {
            var cts = new CancellationTokenSource(100);
            var token = cts.Token;
            var t1 = Task.Run(() => {
                Console.WriteLine("t1 start");
                Task.Delay(220).Wait();
                if (token.IsCancellationRequested) {
                    Console.WriteLine("CancellationRequested");
                    token.ThrowIfCancellationRequested();
                }
                return 1;
            }, token);
            var t2 = t1.ContinueWith((t) => {
                Console.WriteLine("ContinueWith");
                Task.Delay(120).Wait();
                if (token.IsCancellationRequested) {
                    Console.WriteLine("ContinueWith CancellationRequested");
                    //token.ThrowIfCancellationRequested();
                }
            }, token);      //  token 如果是已取消的，t2 根本不会运行

            try {

                t2.Wait();      // 在100毫秒后 token 取消，t2.Wait() 立即返回，也就是这里仅仅只阻塞100毫秒，此时 t1 还没有运行结束
            } catch (Exception ex) {
                Console.WriteLine($"{ex.GetType()}, {ex.Message}");
                // throw;
            }
            Console.WriteLine($"t1.Status: {t1.Status}");
            Console.WriteLine($"t2.Status: {t2.Status}\n\n");

            try {
                // var a = t1.Result;               // t1.Result 会抛出 AggregateException，不会抛出 OperationCanceledException
                // t1.Wait();                       // 和 t1.Result 处理方式一样
                t1.GetAwaiter().GetResult();        // 如果 Task 是取消的，会抛出 TaskCanceledException
                // await t1.Result;                 // 和 t1.GetAwaiter().GetResult() 处理方式一样
            } catch (OperationCanceledException ex) {
                Console.WriteLine("t1 OperationCanceledException");
            } catch (AggregateException ex) {
                Console.WriteLine($"{string.Join("\n", ex.InnerExceptions)}");
            }
            Console.WriteLine($"t1.Status: {t1.Status}");
        }

        public static void TaskCancel2() {
            Random rnd = new Random();
            var cts = new CancellationTokenSource(5000);
            CancellationToken token = cts.Token;

            var t = Task.Run(() => {
                List<int> product33 = new List<int>();
                for (int i = 1; i < Int16.MaxValue; i++) {
                    if (token.IsCancellationRequested) {
                        Console.WriteLine("\nCancellation requested in antecedent...\n");
                        token.ThrowIfCancellationRequested();
                    }
                    if (i % 2000 == 0) { Thread.Sleep(rnd.Next(16, 501)); }
                    if (i % 33 == 0) { product33.Add(i); }
                }

                Console.WriteLine($"end({product33.Count})");
                return product33;
            }, token);

            var continuation = t.ContinueWith(antecedent => {
                var arr = antecedent.Result;
                Console.WriteLine($"Multiples of 33, ({arr.Count}):\n");
                for (int i = 0; i < arr.Count; i++) {
                    if (token.IsCancellationRequested) {
                        Console.WriteLine("\nCancellation requested in continuation...\n");
                        token.ThrowIfCancellationRequested();
                    }

                    if (i % 100 == 0) { Thread.Sleep(rnd.Next(16, 251)); }
                    Console.Write($"{arr[i]}, ");
                }
                Console.WriteLine();
            }, token);

            try {
                continuation.Wait();
            } catch (AggregateException e) {
                foreach (Exception ie in e.InnerExceptions)
                    Console.WriteLine("{0}: {1}", ie.GetType().Name, ie.Message);
            } finally {
                cts.Dispose();
            }

            Console.WriteLine("\nAntecedent Status: {0}", t.Status);
            Console.WriteLine("Continuation Status: {0}", continuation.Status);
        }

        public static void Task_AttachedToParent() {
            var parent = new Task<int[]>(() => {
                Console.WriteLine("parent start" + DateTime.Now.ToLongTimeString());
                var ls = new int[3];

                new Task(() => {
                    Thread.Sleep(1000);
                    ls[0] = 1000;
                }, TaskCreationOptions.AttachedToParent)
                .Start();
                new Task(() => {
                    Thread.Sleep(1500);
                    ls[1] = 1500;
                    Console.WriteLine("1 --" + DateTime.Now.ToLongTimeString());
                }, TaskCreationOptions.AttachedToParent)
                .Start();
                new Task(() => {
                    Thread.Sleep(2000);
                    ls[2] = 2000;
                    Console.WriteLine("2 --" + DateTime.Now.ToLongTimeString());
                }, TaskCreationOptions.AttachedToParent)
                .Start();

                // TaskCreationOptions.AttachedToParent 附加到当前 Task，当前 Task 不会认为已经执行完毕了，
                // 还需要这三个Task 执行完毕才返回结果
                return ls;
            });

            parent.ContinueWith((t) => Console.WriteLine(string.Join(",", t.Result)));
            parent.Start();

            Console.WriteLine("Task_AttachedToParent Return ---" + DateTime.Now.ToLongTimeString());

            // 附加子任务有以下特点：
            // 父级将等待子任务完成, 父级将传播由子任务引发的异常, 父级的状态取决于子级的状态
        }

        public static void Task_AttachedToParent2() {
            Console.WriteLine(DateTime.Now.TimeOfDay);
            var t1 = Task.Run(() => {
                var child = Task.Factory.StartNew(
                    () => {
                        Task.Delay(2000).Wait();
                        Console.WriteLine($"Factory.StartNew, {DateTime.Now.TimeOfDay}");
                    }, TaskCreationOptions.AttachedToParent);
                Console.WriteLine($"child.CreationOptions: {child.CreationOptions}");
                // 这里附加是失败的，但是没有抛出异常，仍会运行任务，但是和父任务没有关联
                // 默认情况，new Task 和 Task.Factory.StartNew 创建的 Task 的 CreationOptions 是 None

                Task.Delay(1000).Wait();
                Console.WriteLine($"Task.Run, {DateTime.Now.TimeOfDay}");
            });
            Console.WriteLine($"t1.CreationOptions: {t1.CreationOptions}");
            t1.Wait();      //  阻塞 1000 毫秒
            Console.WriteLine(DateTime.Now.TimeOfDay);
        }

        public static async Task Task6() {
            var num = 0;

            Action act = async () => {
                Console.WriteLine($"act id: {Task.CurrentId}, {Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(500);
                Console.WriteLine($"act await id: {Task.CurrentId}, {Thread.CurrentThread.ManagedThreadId}");
                num = 3;
            };

            Console.WriteLine($"fn id: {Task.CurrentId}, {Thread.CurrentThread.ManagedThreadId}");
            await Task.Run(act);
            Console.WriteLine($"fn2 id: {Task.CurrentId}, {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine(num);
            // 把 act 的类型改为 Func<Task>，这是这个函数的实际类型，act 实际上返回的是Task，
            // 但是在 await Task.Run(act) 的时候，act 已经执行完毕，并返回一个 Task，应该是被包了一层，没有进行 await

        }

        public static async Task TaskTest() {
            Console.WriteLine($"Function ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            var t1 = Task.Run(() => {
                Console.WriteLine($"Task ThreadId: {Thread.CurrentThread.ManagedThreadId}");
                return DateTime.Now;
            });
            var d = await t1;
            Console.WriteLine($"await ThreadId: {Thread.CurrentThread.ManagedThreadId}");
        }


        #endregion

        #region Parallel

        /// <summary>
        /// Parallel.For    Break   Stop
        /// </summary>
        /// <returns></returns>
        public DoXmIg Parallel1() {
            Parallel.For(10, 50, (i, state) => {
                var info = $"i:{i}, TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}";
                if (i > 30) {
                    Console.Write("!!!!Break, " + info);
                    state.Break();          //结束当前线程的下一次循环, 不会跳出当前块的代码, 后面的代码还是会运行, 不像 Break 关键字有控制代码块的效果

                    //Console.Write("!!!!Stop, " + info);
                    //state.Stop();           // 所有的线程都不会进行下一次循环
                } else {
                    Thread.Sleep(100);
                    Console.Write("----" + info);
                }

                Console.WriteLine(" [End]");
                Thread.Sleep(200);
            });


            return this;
        }

        /// <summary>
        /// Parallel.For    localInit    localFinally
        /// </summary>
        /// <returns></returns>
        public DoXmIg Parallel2() {
            Parallel.For(0, 110_000_000
                , () => {
                    var info = $"TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}";
                    //Console.WriteLine("线程的初始化," + info);
                    return 1000;
                }
                , (i, state, local) => {
                    var info = $"TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}";
                    //Console.WriteLine(i.ToString() + ", " + info);
                    local += 1;
                    //Thread.Sleep(10);

                    return local;
                }
                , (local) => {
                    var info = $"TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}";
                    Console.WriteLine($"TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}, local;{local}");
                });

            return this;
        }

        public DoXmIg Parallel3() {

            Parallel.ForEach(new[] { "AA", "BB", "CC" }, (str, state, i) => {
                Console.WriteLine(str + i.ToString());
            });

            // 并行多个 Action()
            Parallel.Invoke(
                () => Console.WriteLine($"---------- TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}")
                , () => Console.WriteLine($"++++++++++ TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}")
                , () => Console.WriteLine($"========== TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}")
                , () => Console.WriteLine($",,,,,,,,,, TaskId:{Task.CurrentId}, ThreadId:{Thread.CurrentThread.ManagedThreadId}"));

            return this;
        }

        public DoXmIg Parallel4() {
            Console.WriteLine("==========");
            var arr = new List<int>();
            Parallel.ForEach(Enumerable.Range(1, 10_000_000), () => 0,
                (i, state, sum) => {
                    return sum + (i / 2 == 0 ? 1 : 3);
                }, (sum) => {
                    arr.Add(sum);
                });
            Console.WriteLine("==========");
            Console.WriteLine($"{arr.Count}, {arr.Sum()}");
            var sum2 = 0;
            for (var i = 1; i <= 10_000_000; i++) {
                sum2 += i / 2 == 0 ? 1 : 3;
            }
            Console.WriteLine(sum2);
            System.Collections.Concurrent.ConcurrentBag<int> ls;
            return this;
        }

        public static void ParallelExc() {
            try {
                Parallel.For(0, 300, i => {
                    Task.Delay(100).Wait();
                    if (i % 10 == 4) {
                        Console.WriteLine($"OperationCanceledException:{ Thread.CurrentThread.ManagedThreadId }, i: {i}");
                        throw new OperationCanceledException($"{i}");   // 这个异常会进入 AggregateException，无法直接捕获，详见 ParallelCancel
                    }
                    if (i > 70) {
                        Console.WriteLine($"InvalidOperationException:{ Thread.CurrentThread.ManagedThreadId }, i: {i}");
                        throw new InvalidOperationException($"{i}");
                    }
                    // 一个线程出现异常，其他线程也会尽快中断
                    Console.WriteLine($"thread id: { Thread.CurrentThread.ManagedThreadId }, i: {i}");
                });
            } catch (OperationCanceledException e) {        // 无法直接捕获 throw new OperationCanceledException，详见 ParallelCancel
                Console.WriteLine("OperationCanceledException");
            } catch (AggregateException e) {
                Console.WriteLine($"count: {e.InnerExceptions.Count}; {string.Join(",", e.InnerExceptions.Select(ex => ex.Message))}");
                // throw;
            }
        }

        public static void ParallelCancel() {
            var po = new ParallelOptions();
            var cts = new CancellationTokenSource();
            var cts2 = new CancellationTokenSource();
            po.CancellationToken = cts.Token;
            try {
                Parallel.For(0, 500, po, i => {
                    po.CancellationToken.ThrowIfCancellationRequested();
                    Task.Delay(10).Wait();
                    Console.WriteLine($"thread id: { Thread.CurrentThread.ManagedThreadId }, i: {i}");

                    if (i / 100 == 4) {
                        Console.WriteLine($"thread id: { Thread.CurrentThread.ManagedThreadId },\t\t\t cts.Cancel: {i}");
                        cts.Cancel();
                        // 如果直接抛出 OperationCanceledException 会进入 AggregateException
                        // 一个线程取消，其他线程也会尽快的取消，结束执行
                        return;
                    }
                    if (i > 50) {
                        //Console.WriteLine($"thread id: { Thread.CurrentThread.ManagedThreadId },\t\t\t cts2.Cancel: {i}");
                        cts2.Cancel();          // 对该 Parallel 执行无影响
                        return;
                    }
                });
            } catch (OperationCanceledException e) {
                Console.WriteLine("OperationCanceledException");
            } catch (AggregateException e) {
                Console.WriteLine($"count: {e.InnerExceptions.Count}; {string.Join(",", e.InnerExceptions.Select(ex => ex.Message))}");
                // throw;
            }

        }

        public static void ParallelPartitioner() {
            Parallel.ForEach(Partitioner.Create(0, 1000), (range) => {
                // 每一块范围都调用一次委托，而不是每次循环都去调用委托
                var sum = 0;
                for (var i = range.Item1; i < range.Item2; i++) {
                    sum += i;
                }
                Console.WriteLine($"{range}, {sum}");
            });
        }

        #endregion

        public DoXmIg Timer1() {
            var t1 = new Timer(o => Console.WriteLine(o), "ss", 1000 * 10, 1000 * 3);
            // t1 对象如果被垃圾回收，回调将不会执行，所以要有一个变量或成员保持 Timer 的存活
            return this;
        }

        public DoXmIg Timer2() {
            Timer t2 = null;        // 必须提前声明赋值，闭包报错
            t2 = new Timer((o) => {
                t2.Change(1000 * 3, -1);        // 运行正常，但不知道会不会可能为 null，然后出错
                Console.WriteLine(DateTime.Now.ToLongTimeString());
            }, null, 0, -1);
            return this;
        }

        #region Task StateMachine
        // 抄录自《CLR via C#》 28.4

        internal sealed class Type1 { }

        internal sealed class Type2 { }

        private static async Task<Type1> Method1Async() {
            await Task.Delay(1000);
            return new Type1();
        }

        private static async Task<Type2> Method2Async() {
            await Task.Delay(300);
            return new Type2();
        }

        public static async Task<string> TaskAsync1(int argument) {
            int locat = argument;
            try {
                Type1 result1 = await Method1Async();
                for (var x = 0; x < 3; x++) {
                    Type2 result2 = await Method2Async();
                }
            } catch (Exception) {
                Console.WriteLine("Catch");
            } finally {
                Console.WriteLine("Finally");
            }
            return "Done";
        }

        // IL逆向，转换的代码
        [DebuggerStepThrough, AsyncStateMachine(typeof(StateMachine))]
        public static Task<string> TaskAsync2(int argument) {
            StateMachine stateMachine = new StateMachine() {
                m_builder = AsyncTaskMethodBuilder<string>.Create(),
                m_state = -1,
                m_argument = argument
            };

            stateMachine.m_builder.Start(ref stateMachine);
            return stateMachine.m_builder.Task;
        }

        struct StateMachine : System.Runtime.CompilerServices.IAsyncStateMachine {
            public AsyncTaskMethodBuilder<string> m_builder;
            public int m_state;
            public int m_argument, m_local, m_x;
            public Type1 m_resultType1;
            public Type2 m_resultType2;

            private TaskAwaiter<Type1> m_awaiterType1;
            private TaskAwaiter<Type2> m_awaiterType2;

            void IAsyncStateMachine.MoveNext() {
                string result = null;

                // 编译器插入 try 块来确保状态机的任务完成，很有可能程序员编写的代码抛出异常，
                try {
                    bool executeFinally = true;
                    if (m_state == -1) { m_local = m_argument; }

                    // 开始插入程序员编写的原始代码
                    // 原始代码中的 Try 块
                    try {
                        TaskAwaiter<Type1> awaiterType1;
                        TaskAwaiter<Type2> awaiterType2;

                        switch (m_state) {
                        case -1:
                            awaiterType1 = Method1Async().GetAwaiter();
                            if (!awaiterType1.IsCompleted) {
                                m_state = 0;    // 原始代码要以异步的方式完成
                                m_awaiterType1 = awaiterType1;
                                m_builder.AwaitUnsafeOnCompleted(ref awaiterType1, ref this);

                                executeFinally = false;
                                return;
                            }
                            // else 原始代码以同步的方式完成，因为后面会执行 GetResult()，会阻塞当前线程
                            break;
                        case 0:     // 原始代码以异步的方式完成了
                            awaiterType1 = m_awaiterType1;      // 恢复最新的 awaiter
                            break;
                        case 1:
                            awaiterType2 = m_awaiterType2;
                            goto ForLoopEpilog;
                        }

                        m_resultType1 = awaiterType1.GetResult();

                        ForLoopPrologue:
                        m_x = 0;
                        goto ForLoopBody;

                        ForLoopEpilog:
                        m_resultType2 = awaiterType2.GetResult();
                        m_x++;

                        ForLoopBody:
                        if (m_x < 3) {
                            awaiterType2 = Method2Async().GetAwaiter();
                            if (!awaiterType2.IsCompleted) {
                                m_state = 1;
                                m_awaiterType2 = awaiterType2;

                                m_builder.AwaitUnsafeOnCompleted(ref awaiterType2, ref this);
                                executeFinally = false;
                                return;
                            }
                            goto ForLoopEpilog;
                        }

                    } catch (Exception) {
                        Console.WriteLine("Catch");
                    } finally {
                        // 离开了原始代码的 try 块，就执行这里的 finally
                        if (executeFinally) {
                            Console.WriteLine("Finally");
                        }
                    }
                    result = "Done";
                } catch (Exception ex) {
                    // 未处理的异常：通过设置异常来完成状态机的 Task
                    m_builder.SetException(ex);
                    return;
                }
                m_builder.SetResult(result);
            }

            void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine) => throw new NotImplementedException();
        }

        #endregion

        public static void TaskWhenAll() {
            var tasks = Task.WhenAll(new[] {
                Task.Run(()=>{
                    Task.Delay(1000).Wait();
                    Console.WriteLine("11111111");
                }),
                Task.Run(()=>{
                    Task.Delay(200).Wait();
                    Console.WriteLine("2222222222");
                }),
                new Task(()=>{
                    Task.Delay(300).Wait();
                    Console.WriteLine("3333333");
                }),
            });
            tasks.ContinueWith(t => {
                Console.WriteLine("都完成了");
            });
            Console.WriteLine("TaskWhenAll end");
        }

        #region Lock
        // 来自 《CLR via C#》，29，30

        class ThreadSharingData {
            int value;
            int flag;
            void Thread1() {
                value = 3;
                //flag = 1;
                // 在 flag 写入之前，之前的值必须先写入
                Volatile.Write(ref flag, 1);
            }

            void Thread2() {
                // flag 读取之后，之后的值才读取出来
                if (Volatile.Read(ref flag) == 1) {
                    Console.WriteLine(value);
                }
            }
        }

        class MultiWebRequests {
            AsyncCoordinator ac = new AsyncCoordinator();
            Dictionary<string, object> servers = new Dictionary<string, object> {
                {"http://Microsoft.com", null },
                {"http://bing.com", null },
                {"http://baidu.com", null },
            };

            public MultiWebRequests(int timeout = Timeout.Infinite) {
                var httpClient = new HttpClient();
                foreach (var url in servers.Keys) {
                    ac.AboutToBegin(1);
                    httpClient.GetByteArrayAsync(url)
                        .ContinueWith(t => ComputeResult(url, t));
                }
                ac.AllBegun(AllDone, timeout);
            }

            private void ComputeResult(string url, Task<byte[]> task) {
                object result;
                if (task.Exception != null) {
                    result = task.Exception.InnerException;
                } else {
                    // 在线程池线程处理I/O
                    // 在此处理数据
                    result = task.Result.Length;
                }
                servers[url] = result;
                ac.JustEnded();
            }

            public void Cancel() => ac.Cancel();

            private void AllDone(CoordinationStatus status) {
                switch (status) {
                case CoordinationStatus.Cancel:
                    Console.WriteLine("Cancel");
                    break;
                case CoordinationStatus.Timeout:
                    Console.WriteLine("Timeout");
                    break;
                case CoordinationStatus.AllDone:
                    Console.WriteLine("All Done");
                    foreach (var server in servers) {
                        Console.Write($"{server.Key} ");
                        if (server.Value is Exception ex) {
                            Console.WriteLine($"failed due to {ex.GetType().Name}.");
                        } else {
                            Console.WriteLine($"returned {server.Value:N0} byte");
                        }
                    }
                    break;
                }
            }
        }
        class AsyncCoordinator {
            int opCount = 1;        // 内部初始为 1，AllBegun方法内部减 1
            int statusReported;     // 作为 bool 变量使用，因为 Interlocked.Exchange 没有 bool 的重载
            Action<CoordinationStatus> callback;
            Timer timer;

            // 在进行操作之前调用
            public void AboutToBegin(int opsToAdd) {
                Interlocked.Add(ref opCount, opsToAdd);
            }

            // 处理好一个操作之后调用
            public void JustEnded() {
                if (Interlocked.Decrement(ref opCount) == 0) {
                    ReportStatus(CoordinationStatus.AllDone);
                }
            }

            private void ReportStatus(CoordinationStatus status) {
                if (Interlocked.Exchange(ref statusReported, 1) == 0) {
                    callback?.Invoke(status);       //仅仅调用一次回调
                }
            }
            private void ReportStatus2(CoordinationStatus status) {
                if (statusReported == 1) { return; }        // 这么写是有问题的，当 statusReported == 0，两个线程可以同时执行后面的代码，疑问中？待确定
                Interlocked.Exchange(ref statusReported, 1);
                callback?.Invoke(status);
            }

            // 必须在所有操作之后调用
            public void AllBegun(Action<CoordinationStatus> callback, int timeout = Timeout.Infinite) {
                this.callback = callback;
                if (timeout != Timeout.Infinite) {
                    this.timer = new Timer(TimeExpired, null, timeout, Timeout.Infinite);
                }
                JustEnded();
            }

            private void TimeExpired(object obj) => ReportStatus(CoordinationStatus.Timeout);

            public void Cancel() => ReportStatus(CoordinationStatus.Cancel);

        }
        enum CoordinationStatus {
            Cancel, Timeout, AllDone
        }

        // 乐观并发模式创建一个原子 Maximum
        public static int Maximum(ref int target, int value) {
            int currentVal = target, startVal, desiredVal;

            do {
                startVal = currentVal;
                desiredVal = Math.Max(startVal, value);

                // 不是原子性的
                if (target == startVal) target = desiredVal;

                currentVal = Interlocked.CompareExchange(ref target, desiredVal, startVal);
            } while (startVal != currentVal);

            return desiredVal;
        }

        class SomeCalss1 : IDisposable {
            readonly Mutex mutex = new Mutex();

            public void Method1() {
                mutex.WaitOne();
                // do something
                Method2();          // Method2 递归获取锁
                mutex.ReleaseMutex();
            }

            public void Method2() {
                mutex.WaitOne();
                // do something
                mutex.ReleaseMutex();
            }

            public void Dispose() => mutex.Dispose();
        }

        public sealed class RecursiveAutoResetEvent : IDisposable {
            AutoResetEvent arLock = new AutoResetEvent(true);
            int owningThreadId = 0;
            int recursionCount = 0;

            public void Enter() {
                // 获取调用线程的唯一Id
                int id = Thread.CurrentThread.ManagedThreadId;
                // 调用线程拥有锁，增加递归计数
                if (owningThreadId == id) {
                    recursionCount++;
                    return;
                }
                // 调用线程不拥有锁，等待它
                arLock.WaitOne();
                // 调用线程现在才拥有锁，初始化计数
                owningThreadId = id;
                recursionCount = 1;
            }

            public void Leave() {
                // 调用线程不拥有锁，报错
                if (owningThreadId != Thread.CurrentThread.ManagedThreadId) {
                    throw new InvalidOperationException();
                }

                if (--recursionCount == 0) {
                    owningThreadId = 0;     // 表明没有线程占用锁，清零
                    arLock.Set();           // 唤醒一个正在等待的线程（如果有）
                }
            }

            public void Dispose() => arLock.Dispose();
        }

        public sealed class SimpleHybridLock : IDisposable {
            // 由基元用户模式构造（Interlocked 的方法）使用
            int waiters = 0;
            // AutoResetEvent 是基元内核模式构造
            AutoResetEvent evLock = new AutoResetEvent(false);

            public void Enter() {
                if (Interlocked.Increment(ref waiters) == 1) { return; }

                // 发生竞争，这个线程等待
                evLock.WaitOne();       // 这里产生较大的性能影响
                // WaitOne 返回后，这个线程拿到了锁
            }

            public void Leave() {
                if (Interlocked.Decrement(ref waiters) <= 0) { return; }

                // 有其他线程正在阻塞，唤醒其中一个
                evLock.Set();   // 这里产生较大的性能影响
            }

            public void Dispose() => evLock.Dispose();
        }

        public sealed class AnotherHybridLock : IDisposable {
            int waiters = 0;    // 由基元用户模式构造（Interlocked 的方法）使用
            AutoResetEvent evLock = new AutoResetEvent(false);      // AutoResetEvent 是基元内核模式构造
            int spinCount = 4000;
            int owningThreadId = 0;
            int recursion = 0;


            public void Enter() {
                int id = Thread.CurrentThread.ManagedThreadId;
                // 调用线程已经拥有锁，增加递归计数
                if (id == owningThreadId) { owningThreadId++; return; }

                // 调用线程不拥有锁
                var spin = new SpinWait();
                for (int i = 0; i < spinCount; i++) {
                    // 自旋过程中，锁没有被占用，这个线程就获得锁
                    if (Interlocked.CompareExchange(ref waiters, 1, 0) == 0) { goto GotLock; }
                    spin.SpinOnce();    // 黑科技：给其他线程运行的机会，希望锁会被释放
                }

                // 自旋结束，锁仍未获得，再试一次
                if (Interlocked.Increment(ref waiters) > 1) {
                    // 仍然是竞态条件，阻塞这个线程
                    evLock.WaitOne();   // 等待锁，性能有损失
                    // 线程醒来时，这个线程获取锁
                }

                GotLock:
                owningThreadId = id;    // 重置
                recursion = 1;
            }

            public void Leave() {
                int id = Thread.CurrentThread.ManagedThreadId;
                if (id != owningThreadId) {
                    throw new SynchronizationLockException("Lock not owned by calling thread");
                }

                // 这个线程可能存在递归计数
                if (--recursion > 0) { return; }

                owningThreadId = 0;     // 现在没有线程拥有锁
                if (Interlocked.Decrement(ref waiters) == 0) { return; }

                // 有其他线程在等待，唤醒一个
                evLock.Set();   //这里有较大的性能损失
            }

            public void Dispose() => evLock.Dispose();
        }

        public sealed class Transaction : IDisposable {
            readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
            DateTime timeOfLastTrans;

            public void PerformTransaction(int sleep) {
                rwLock.EnterWriteLock();
                Thread.Sleep(sleep);
                timeOfLastTrans = DateTime.Now;
                Console.WriteLine($"({Thread.CurrentThread.ManagedThreadId})write: {timeOfLastTrans}");
                rwLock.ExitWriteLock();
            }

            public DateTime LastTransaction {
                get {
                    Thread.Sleep(1000);
                    rwLock.EnterReadLock();
                    var val = timeOfLastTrans;
                    Console.WriteLine($"({Thread.CurrentThread.ManagedThreadId})read: {timeOfLastTrans}");
                    rwLock.ExitReadLock();
                    return val;
                }
            }

            public static void Test1() {
                using (var tran = new Transaction()) {
                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()}");

                    Action readInfo = () => Console.WriteLine($"({Thread.CurrentThread.ManagedThreadId}){DateTime.Now.ToLongTimeString()} -- {tran.LastTransaction}");

                    Parallel.Invoke(
                        () => tran.PerformTransaction(3000),
                        () => tran.PerformTransaction(2000),
                        () => tran.PerformTransaction(3000),
                        readInfo, readInfo, readInfo
                    );
                }
            }

            public static void Test2() {
                using (var tran = new Transaction()) {
                    Console.WriteLine($"({Thread.CurrentThread.ManagedThreadId}){DateTime.Now.ToLongTimeString()}");
                    Action readInfo = () => Console.WriteLine($"({Thread.CurrentThread.ManagedThreadId}){DateTime.Now.ToLongTimeString()} -- {tran.LastTransaction}");
                    tran.PerformTransaction(1000);
                    readInfo();
                    Task.Run(readInfo);
                    Task.Run(readInfo);
                    Task.Run(() => tran.PerformTransaction(4000));
                    Task.Run(readInfo);
                    readInfo();
                }
            }

            public void Dispose() => rwLock.Dispose();
        }

        public enum OneManyMode {
            Exclusive, Shared
        }
        public sealed class AsyncOneManyLock {
            #region 锁的代码
            SpinLock spinLock = new SpinLock();
            void Lock() {
                var token = false;
                spinLock.Enter(ref token);
            }
            void UnLock() => spinLock.Exit();

            #endregion

            #region 锁的状态和辅助方法
            int state = 0;
            bool isFree => state == 0;
            bool isOwnedByWriter => state == -1;
            bool isOwnedByreaders => state > 0;
            int addReaders(int count) => state += count;
            int subtractReaders() => --state;
            void makeWriter() => state = -1;
            void makeFree() => state = 0;
            #endregion

            readonly Task noContentionAccessGranter = Task.FromResult<object>(null);
            readonly Queue<TaskCompletionSource<object>> waitingWriters = new Queue<TaskCompletionSource<object>>();
            TaskCompletionSource<object> waitingReadersSignal = new TaskCompletionSource<object>();
            int waitingReadersCount;

            public Task WaitAsync(OneManyMode mode) {
                var accessGranter = noContentionAccessGranter;

                Lock();
                switch (mode) {
                case OneManyMode.Exclusive:
                    if (isFree) { makeWriter(); break; }
                    var tcs = new TaskCompletionSource<object>();
                    waitingWriters.Enqueue(tcs);
                    accessGranter = tcs.Task;
                    break;
                case OneManyMode.Shared:
                    if (isFree || (isOwnedByreaders && waitingWriters.Count == 0)) {
                        addReaders(1);
                        break;
                    }
                    waitingReadersCount++;
                    accessGranter = waitingReadersSignal.Task.ContinueWith(t => t.Result);
                    break;
                }
                UnLock();

                return accessGranter;
            }

            public void Release() {
                TaskCompletionSource<object> accessGranter = null;

                Lock();
                if (isOwnedByWriter) {
                    makeFree();
                } else {
                    subtractReaders();
                }

                if (isFree) {
                    if (waitingWriters.Count > 0) {
                        makeWriter();
                        accessGranter = waitingWriters.Dequeue();
                    } else if (waitingReadersCount > 0) {
                        addReaders(waitingReadersCount);
                        waitingReadersCount = 0;
                        accessGranter = waitingReadersSignal;
                        waitingReadersSignal = new TaskCompletionSource<object>();
                    }
                }
                UnLock();
                if (accessGranter != null) {
                    accessGranter.SetResult(null);  // 唤醒外面的 waiter/reader，减少竞争几率以提高性能
                }
            }

        }

        public static class TestSpinLock {
            static SpinLock spinLock = new SpinLock();
            public static int Num;

            public static void Test() {
                Parallel.Invoke(Thread1, Thread2);
            }
            static void Thread1() {
                Console.WriteLine("Thread1 ----------");
                var token = false;
                spinLock.Enter(ref token);      // 有一个 spinLock 在自旋的时候，其他的 spinLock.Enter 也不能进入代码快运行
                Console.WriteLine("Thread1 Enter");
                Task.Delay(1000).Wait();
                Num += 10;
                Console.WriteLine($"Thread1 Exit, Num: {Num}");
                spinLock.Exit();
            }
            static void Thread2() {
                Console.WriteLine("Thread2 ----------");
                var token = false;
                spinLock.Enter(ref token);      // 有一个 spinLock 在自旋的时候，其他的 spinLock.Enter 也不能进入代码快运行
                Console.WriteLine("Thread2 Enter");
                Task.Delay(2000).Wait();
                Num += 1000;
                Console.WriteLine($"Thread2 Exit, Num: {Num}");
                spinLock.Exit();
            }

        }

        public static class TestMonitor {
            static object objLock = new object();

            public static void Test() {
                Parallel.Invoke(Thread1, Thread2);
            }
            static void Thread1() {
                Console.WriteLine("Thread1 ----------");
                var token = false;
                Monitor.Enter(objLock, ref token);      // objLock 被锁住的时候，其他的代码块也不能进入
                Console.WriteLine("Thread1 Enter");
                Task.Delay(1000).Wait();
                Console.WriteLine($"Thread1 Exit");
                Monitor.Exit(objLock);
            }
            static void Thread2() {
                Console.WriteLine("Thread2 ----------");
                var token = false;
                Monitor.Enter(objLock, ref token);      // objLock 被锁住的时候，其他的代码块也不能进入
                Console.WriteLine("Thread2 Enter");
                Task.Delay(2000).Wait();
                Console.WriteLine($"Thread2 Exit");
                Monitor.Exit(objLock);
            }
        }
        #endregion

        #region DataFlow

        public class DF {
            public static void Test1() {

            }

            public static void BufferBlock1() {
                var buffer = new BufferBlock<int>();
                Task.Run(Receive);
                buffer.Post(1);
                buffer.Post(12);
                buffer.Post(13);
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} --- {buffer.Count}");
                buffer.Post(2);
                Task.Delay(10).Wait();
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} --- {buffer.Count}");
                buffer.Post(3);
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} --- {buffer.Count}");

                async Task Receive() {
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}, {await buffer.ReceiveAsync()}");
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}, {await buffer.ReceiveAsync()}");
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}, {await buffer.ReceiveAsync()}");
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}, {await buffer.ReceiveAsync()}");
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}, {await buffer.ReceiveAsync()}");
                }
            }

            public static void ActionBlock1() {
                var act = new ActionBlock<int>((item) => {
                    Console.WriteLine(item);
                });
            }

        }


        #endregion
    }

}
