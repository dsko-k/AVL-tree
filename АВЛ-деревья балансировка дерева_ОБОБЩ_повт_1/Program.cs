using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

/*
Реализовать АВЛ-дерево, создав классы и следующие методы с применением обобщений для работы 
с любым типом ключей:

1. Вставки элементов
2. Обхода дерева: 
    2.1. в ширину
    2.2. в глубину (рекурсией)
3. Удаление узла (3 случая, в том числе, когда узел имеет левого и правого потомков)
4. Перечень несбалансированных потомков заданного узла и сам заданный узел, если он не сбалансирован
5. Балансировка всего дерева (перестроение узлов и приведение к АВЛ-дереву)
*/


namespace АВЛ_деревья_балансировка_дерева_ОБОБЩ_повт_1
{
    abstract class TreeMethod<T> 
    {
        public abstract void Add(T value); // 1. Добавление узла
        public abstract void VisitWidth(); // 2.1. Обход дерева в ширину

        public abstract void VisitDepth(); // 2.2. Обход дерева в глубину


        // 3. Удаление узла (3 случая, в том числе, когда узел имеет левого и правого потомков). Удаляется первый узел из списка найденых
        public abstract void Delete(T node); 


        // 4. Перечень несбалансированных потомков узла node и сам узел node, если он не сбалансирован
        public abstract Queue<Node<T>> AllNonBalanced<T>(Node<T> node) where T: IComparable; 

        public abstract void Balance(); // 5. Балансировка всего дерева


        // Информация об узле
        public abstract void Message<T>(Node<T> value, bool? is_left, string message) where T : IComparable; 
    }


    // Класс, описывающий узел дерева
    class Node<T> : IComparable<T> where T: IComparable
    {       
        public Node(T Value)
        {
            Parent = null;      // ссылка на родительский узел
            IsLeftChild = null; // является ли узел левым потомком своего родителя
            NodeValue = Value;  // значение узла

            LeftChild = null;   // ссылка на левого потомка узла
            RightChild = null;  // ссылка на правого потомка узла

            IsVisited = false;  // посещён ли узел
        }


        public Node<T> Parent { get; set; }

        public bool? IsLeftChild { get; set; }

        public T NodeValue { get; set; }

        public Node<T> LeftChild { get; set; }
        public Node<T> RightChild { get; set; }

        public bool IsVisited { get; set; }


        public int CompareTo(T other)
        {
            return other.CompareTo(NodeValue);
        }
        
    }


    // Класс, описывающий дерево
    class Tree<T> : TreeMethod<T> where T : IComparable
    {
       
        Node<T> head = null;

        Node<T> parent = null;
        Node<T> current = null;

        string nullValue = "  ";


        // Добавление узла
        public override void Add(T value)
        {
            Title("Добавление узла: метод Add(T value)", ConsoleColor.Magenta, true);

            List<Node<T>> foundNodes = new List<Node<T>>();


            if (head == null)
            {
                head = new Node<T>(value);
                current = head;

                foundNodes.Add(head);

                Message(head, false, "Добавлен: ");
            }
            else
            {
                current = head;

                while (current != null)
                {

                    if (value.CompareTo(current.NodeValue) == -1)
                    {
                        parent = current;
                        current = current.LeftChild;
                    }
                    else
                    {
                        parent = current;
                        current = current.RightChild;
                    }
                }


                if (value.CompareTo(parent.NodeValue) == -1)
                {
                    parent.LeftChild = new Node<T>(value);

                    parent.LeftChild.Parent = parent;


                    Message(parent.LeftChild, true, "Добавлен: ");

                    parent.LeftChild.IsLeftChild = true;

                    
                    foundNodes.Add(parent.LeftChild);
                }
                else
                {
                    parent.RightChild = new Node<T>(value);

                    parent.RightChild.Parent = parent;


                    Message(parent.RightChild, false, "Добавлен: ");

                    parent.RightChild.IsLeftChild = false;


                    foundNodes.Add(parent.RightChild);
                }
                
            }


            string notation = "Добавленый узел со значением " + value + "\n";

            ColorNotification(foundNodes, notation);
        }


        // Обход дерева в ширину
        public override void  VisitWidth()
        {
            Title("Обход дерева в ширину. Метод VisitWidth()", ConsoleColor.Magenta, true);
                        
            Stack<Node<T>> stack = new Stack<Node<T>>();
            Stack<Node<T>> inner_stack = new Stack<Node<T>>();
            
            current = head;

            if (head != null)
            {
                stack.Push(head);

                Message(head, true, "Обход: ");
            }
            

            while (stack.Count > 0)
            {
                foreach (var iter in stack)
                {
                    if (iter.LeftChild != null)
                    {
                        inner_stack.Push(iter.LeftChild);

                        Message(iter.LeftChild, true, "Обход: ");

                    }

                    if (iter.RightChild != null)
                    {
                        inner_stack.Push(iter.RightChild);

                        Message(iter.RightChild, false, "Обход: ");
                    }
                }

                stack.Clear();

                foreach (var iter in inner_stack)
                {
                    if (iter.LeftChild != null)
                    {
                        stack.Push(iter.LeftChild);

                        Message(iter.LeftChild, true, "Обход: ");

                    }

                    if (iter.RightChild != null)
                    {
                        stack.Push(iter.RightChild);

                        Message(iter.RightChild, false, "Обход: ");
                    }
                }

                inner_stack.Clear();
            }

        }

        // Обход дерева в глубину
        public override void VisitDepth() // метод-обёртка
        {
            Title("Обход дерева в ширину. Метод VisitDepth()", ConsoleColor.Magenta, true);

            VisitSubTree(head);
        }



        // Посетить узел (посещает узел и возвращает его родителя)
        Node<T> VisitNode<R>(R node) where R : Node<T>
        {
            //1. Опуститься в *самый нижний левый* узел от узла node
            //2.1 Если у *самого нижнего левого* узла НЕТ *правого потомка*, то обойти 
            // *самый нижний левый узел* и вернуть ближайшего родителя, который ещё не обойдён

            //2.1.2 Если у *самого нижнего левого* узла ЕСТЬ *правый потомок* у которого:

            // нет потомков то, обойти *самый нижний левый узел*
            // есть потомки то, ВЕРНУТЬ *правого потомка*

            current = node;

            if (current != null)
            {

                //1. Опуститься в *самый нижний левый* узел от узла node
                while (current.LeftChild != null && current.LeftChild.IsVisited == false)
                {
                    current = current.LeftChild;
                }

                Message(current, current.IsLeftChild, "Обход вглубину: ");

                current.IsVisited = true;


                //2.1 Если у *самого нижнего левого* узла НЕТ *правого потомка*, то обойти 
                // *самый нижний левый узел* и вернуть ближайшего родителя, который ещё не обойдён

                if (current.RightChild != null)
                {
                    if (!HasChildren(current.RightChild))
                    {
                        Message(current.RightChild, current.RightChild.IsLeftChild, "Обход вглубину: ");

                        current.RightChild.IsVisited = true;
                    }
                    else
                    {
                        return current.RightChild;
                    }
                }

                while (current != null && current.IsVisited)
                {
                    current = current.Parent;
                }

                return current;
            }
            else
            {
                return null;
            }
        }


        // Обход вглубину левого или правого поддерева (вспомогательный рекурсивный метод)
        void VisitSubTree<R>(R node) where R : Node<T>
        {
            current = node;

            if (current == null)
            {
                return;
            }
            else
            {
                VisitSubTree(VisitNode(current));
            }
        }


        // Есть ли у заданного узла потомки
        bool HasChildren<T>(Node<T> node) where T : IComparable
        {

            if (node.LeftChild == null && node.RightChild == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        
        // к п.3. Удаление узла (3 случая, в том числе, когда узел имеет левого и правого потомков)
        // Найти приемника: наименьшее число среди чисел, которые больше удаляемого узла
        Node<T> Successor<R>(R node) where R : Node<T>
        {
            Title("Поиск узла-преемника: мин. число среди чисел, больших удаляемого узла. Метод Successor()", ConsoleColor.White, false);

            Node<T> successor;

            current = node;

            if (current.RightChild != null)
            {
                current = current.RightChild;
            }

            while (current != null)
            {
                if (current.LeftChild != null)
                {
                    current = current.LeftChild;
                }
                else
                {
                    break;
                }
            }

            successor = current;

            Message(successor, successor.IsLeftChild, "Преемник узла " + node.NodeValue.ToString() + ": ");

            return successor;
        }


        // к п.3. Поиск узла с заданным значением
        public List<Node<T>> FindNode(T value)
        {
            Title("Поиск узлов со значением " + value + ". Метод FindNode(T value)", ConsoleColor.Magenta, true);
                        
            current = head;
            Node<T> result = null;

            List<Node<T>> foundNodes = new List<Node<T>>();

            Stack<Node<T>> stack = new Stack<Node<T>>();
            Stack<Node<T>> stackInner = new Stack<Node<T>>();

            if (current != null)
            {
                stack.Push(current);
            }

            if (current.NodeValue.CompareTo(value) == 0)
            {
                result = current;

                foundNodes.Add(result);

                Console.ForegroundColor = ConsoleColor.Green;
                Message(result, result.IsLeftChild, "Найден узел, со значением " + value + " :");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                while (stack.Count > 0)
                {

                    foreach (var iter in stack)
                    {
                        if (iter.LeftChild != null)
                        {
                            stackInner.Push(iter.LeftChild);

                            if (iter.LeftChild.NodeValue.CompareTo(value) == 0)
                            {
                                result = iter.LeftChild;
                                foundNodes.Add(result);

                                Console.ForegroundColor = ConsoleColor.Green;
                                Message(result, result.IsLeftChild, "Найден узел, со значением " + value + " :");
                                Console.ForegroundColor = ConsoleColor.Gray;
                            }
                        }

                        if (iter.RightChild != null)
                        {
                            stackInner.Push(iter.RightChild);

                            if (iter.RightChild.NodeValue.CompareTo(value) == 0)
                            {
                                result = iter.RightChild;
                                foundNodes.Add(result);

                                Console.ForegroundColor = ConsoleColor.Green;
                                Message(result, result.IsLeftChild, "Найден узел, со значением " + value + " :");
                                Console.ForegroundColor = ConsoleColor.Gray;
                            }
                        }
                    }

                    stack.Clear();


                    if (result == null)
                    {
                        foreach (var iter_inner in stackInner)
                        {
                            if (iter_inner.LeftChild != null)
                            {
                                stack.Push(iter_inner.LeftChild);

                                if (iter_inner.LeftChild.NodeValue.CompareTo(value) == 0)
                                {
                                    result = iter_inner.LeftChild;
                                    foundNodes.Add(result);

                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Message(result, result.IsLeftChild, "Найден узел, со значением " + value + " :");
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                }
                            }

                            if (iter_inner.RightChild != null)
                            {
                                stack.Push(iter_inner.RightChild);

                                if (iter_inner.RightChild.NodeValue.CompareTo(value) == 0)
                                {
                                    result = iter_inner.RightChild;
                                    foundNodes.Add(result);

                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Message(result, result.IsLeftChild, "Найден узел, со значением " + value + " :");
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                }
                            }
                        }

                        stackInner.Clear();
                    }

                }
            }


            if (result == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("НЕ найден узел, со значением {0}", value);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            
            string notation = "Найденые узлы со значением " + value + "\n";

            ColorNotification(foundNodes, notation);
            

            return foundNodes;
        }


        // Все потомки узла node и вывод макс. глубины дерева (вспомогательный метод для п.5)        
        Queue<Node<T>> ChildrenNodes<T>(Node<T> node, out int profound) where T : IComparable
        {
            Queue<Node<T>> allChildren = new Queue<Node<T>>();

            int depth = 0;

            Stack<Node<T>> stack = new Stack<Node<T>>();
            Stack<Node<T>> stackInner = new Stack<Node<T>>();

            if (node != null)
            {
                stack.Push(node);
                allChildren.Enqueue(node);
                depth++;


                while (stack.Count > 0)
                {
                    foreach (var iter in stack)
                    {
                        if (iter.LeftChild != null)
                        {
                            stackInner.Push(iter.LeftChild);
                            allChildren.Enqueue(iter.LeftChild);
                        }


                        if (iter.RightChild != null)
                        {
                            stackInner.Push(iter.RightChild);
                            allChildren.Enqueue(iter.RightChild);
                        }
                    }

                    if (stackInner.Count > 0)
                        depth++;


                    stack.Clear();


                    foreach (var iterInner in stackInner)
                    {
                        if (iterInner.LeftChild != null)
                        {
                            stack.Push(iterInner.LeftChild);
                            allChildren.Enqueue(iterInner.LeftChild);
                        }

                        if (iterInner.RightChild != null)
                        {
                            stack.Push(iterInner.RightChild);
                            allChildren.Enqueue(iterInner.RightChild);
                        }
                    }

                    if (stack.Count > 0)
                        depth++;


                    stackInner.Clear();
                }
            }

            profound = depth;

            return allChildren;
        }



        // Одинаковая ли глубина поддеревьев узла node
        bool IsEqualProfound<T>(Node<T> node, out int leftProfound, out int rightProfound) where T : IComparable
        {
            int leftProf = 0; int rightProf = 0;

            Queue<Node<T>> leftSubTree = ChildrenNodes(node.LeftChild, out leftProf);
            Queue<Node<T>> rightSubTree = ChildrenNodes(node.RightChild, out rightProf);

            leftProfound = leftProf;
            rightProfound = rightProf;

            if (Math.Abs(leftProf - rightProf) <= 1)
                return true;

            else
                return false;
        }



        // Перечень несбалансированных потомков узла node и сам узел node, если он не сбалансирован
        public override Queue<Node<T>> AllNonBalanced<T>(Node<T> node)
        {
            Queue<Node<T>> allNonBalanced = new Queue<Node<T>>();

            int leftProf = 0; int rightProf = 0;

            int profound = 0;

            Queue<Node<T>> children = ChildrenNodes(node, out profound);

            // проверка самого узла node

            if (!IsEqualProfound(node, out leftProf, out rightProf))
            {
                allNonBalanced.Enqueue(node);
            }

            // проверка детей узла node
            foreach (var iter in children)
            {
                if (!IsEqualProfound(iter, out leftProf, out rightProf))
                {
                    allNonBalanced.Enqueue(iter);
                }
            }


            return allNonBalanced;
        }


        // Поиск минимального (максимального) узла среди 3-х узлов.  out int index - порядковый номер искомого узла среди x, y, z (отсчёт с 0)
        Node<T> MinMaxNode(Node<T> x, Node<T> y, Node<T> z, bool isMin, out int index)
        {
            Node<T>[] array = new Node<T>[3];

            array[0] = x;
            array[1] = y;
            array[2] = z;

            Node<T> targetNode = null;
            int indexArray = 0;


            if (isMin)
            {
                targetNode = x;

                for (int i = 1; i < array.Length; i++)
                {
                    if (array[i].NodeValue.CompareTo(targetNode.NodeValue) == -1)
                    {
                        targetNode = array[i];
                        indexArray = i;
                    }
                }
            }
            else
            {
                targetNode = z;

                indexArray = 2;

                for (int i = 1; i >= 0; i--)
                {
                    if (array[i].NodeValue.CompareTo(targetNode.NodeValue) == 1)
                    {
                        targetNode = array[i];
                        indexArray = i;
                    }
                }
            }



            index = indexArray;

            //Console.Write("Среди узлов x = {0}, y = {1}, z = {2}  ", x.NodeValue, y.NodeValue, y.NodeValue);

            //if (isMin == true)
            //{
            //    Message(targetNode, targetNode.IsLeftChild, "Минимальный узел: ");
            //}
            //else
            //{
            //    Message(targetNode, targetNode.IsLeftChild, "Максимальный узел: ");
            //}

            return targetNode;
        }


        // Поиск медианного узла среди 3-х узлов. out int index - порядковый номер искомого узла среди x, y, z (отсчёт с 0)
        Node<T> MedianNode(Node<T> x, Node<T> y, Node<T> z, out int index)
        {
            Node<T> medianNode = x;

            Node<T>[] array = new Node<T>[3];

            array[0] = x;
            array[1] = y;
            array[2] = z;

            int indexMin = 0;
            int indexMax = 0;

            MinMaxNode(x, y, z, true, out indexMin);
            MinMaxNode(x, y, z, false, out indexMax);

            int medianIndex = 0;


            for (int i = 0; i < array.Length; i++)
            {
                if (i != indexMin && i != indexMax)
                {
                    medianNode = array[i];
                    medianIndex = i;
                    break;
                }
            }


            index = medianIndex;

            //Console.Write("Среди узлов x = {0}, y = {1}, z = {2}  ", x.NodeValue, y.NodeValue, y.NodeValue);

            //Message(medianNode, medianNode.IsLeftChild, "Медианный узел: ");


            return medianNode;
        }


        // Выдача узлов в порядке: минимальный узел - медианный узел - максимальный узел
        public List<KeyValuePair<int, Node<T>>> MinMidMax(Node<T> x, Node<T> y, Node<T> z)
        {
            Node<T>[] XYZ = new Node<T>[3];
            XYZ[0] = x;
            XYZ[1] = y;
            XYZ[2] = z;

            // <int, Node<T>>    <Ключ: индекс в массиве XYZ, Значение: узел Node<T>>
            List<KeyValuePair<int, Node<T>>> listXYZ = new List<KeyValuePair<int, Node<T>>>();

            int indexMin = 0;
            int indexMax = 0;
            int indexMiddle = 0;

            MinMaxNode(x, y, z, true, out indexMin);
            MinMaxNode(x, y, z, false, out indexMax);
            MedianNode(x, y, z, out indexMiddle);


            listXYZ.Add(new KeyValuePair<int, Node<T>>(indexMin, MinMaxNode(x, y, z, true, out indexMin)));
            listXYZ.Add(new KeyValuePair<int, Node<T>>(indexMiddle, MedianNode(x, y, z, out indexMiddle)));
            listXYZ.Add(new KeyValuePair<int, Node<T>>(indexMax, MinMaxNode(x, y, z, false, out indexMax)));


            return listXYZ;
        }

        // Ребалансировка 3-х узлов x, y, z
        void Rebalance(Node<T> x, Node<T> y, Node<T> z)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\tБудет проведена ребалансировка узлов: x = {0}, y = {1}, z = {2}\n", x.NodeValue, y.NodeValue, z.NodeValue);
            Console.ForegroundColor = ConsoleColor.Gray;

            ColorNotification(new List<Node<T>> { x, y, z }, "Узлы, которые БУДУТ ребалансироваться");


            bool? xIsLeftChild = x.IsLeftChild;

            List<KeyValuePair<int, Node<T>>> minMidMaxXYZ = MinMidMax(x, y, z);

            Node<T> exMiddleLeftNode = null;
            Node<T> exMiddleRightNode = null;

            Node<T>[] deCoupledXYZ = DeCoupled(x, y, z);


            //  новый родитель среднего узла
            if (x.Parent == null)
            {
                head = deCoupledXYZ[minMidMaxXYZ[1].Key];
                deCoupledXYZ[minMidMaxXYZ[1].Key].IsLeftChild = null;
            }
            else
            {
                deCoupledXYZ[minMidMaxXYZ[1].Key].Parent = x.Parent;

                if (xIsLeftChild == true)
                {
                    x.Parent.LeftChild = deCoupledXYZ[minMidMaxXYZ[1].Key];
                    deCoupledXYZ[minMidMaxXYZ[1].Key].IsLeftChild = true;
                }
                else if (xIsLeftChild == false)
                {
                    x.Parent.RightChild = deCoupledXYZ[minMidMaxXYZ[1].Key];
                    deCoupledXYZ[minMidMaxXYZ[1].Key].IsLeftChild = false;
                }
            }


            // потомки среднего узла до ребалансировки
            if (deCoupledXYZ[minMidMaxXYZ[1].Key].LeftChild != null)
            {
                exMiddleLeftNode = deCoupledXYZ[minMidMaxXYZ[1].Key].LeftChild;
            }

            if (deCoupledXYZ[minMidMaxXYZ[1].Key].RightChild != null)
            {
                exMiddleRightNode = deCoupledXYZ[minMidMaxXYZ[1].Key].RightChild;
            }



            // новые потомки среднего узла
            deCoupledXYZ[minMidMaxXYZ[1].Key].LeftChild = deCoupledXYZ[minMidMaxXYZ[0].Key];
            deCoupledXYZ[minMidMaxXYZ[1].Key].RightChild = deCoupledXYZ[minMidMaxXYZ[2].Key];

            deCoupledXYZ[minMidMaxXYZ[0].Key].IsLeftChild = true;
            deCoupledXYZ[minMidMaxXYZ[2].Key].IsLeftChild = false;


            deCoupledXYZ[minMidMaxXYZ[0].Key].Parent = deCoupledXYZ[minMidMaxXYZ[1].Key];

            deCoupledXYZ[minMidMaxXYZ[2].Key].Parent = deCoupledXYZ[minMidMaxXYZ[1].Key];


            // присоединение узлов, которые были потомками среднего узла до ребалансировки

            if (exMiddleLeftNode != null)
            {
                deCoupledXYZ[minMidMaxXYZ[0].Key].RightChild = exMiddleLeftNode;
                exMiddleLeftNode.Parent = deCoupledXYZ[minMidMaxXYZ[0].Key];
                exMiddleLeftNode.IsLeftChild = false;
            }

            if (exMiddleRightNode != null)
            {
                deCoupledXYZ[minMidMaxXYZ[2].Key].LeftChild = exMiddleRightNode;
                exMiddleRightNode.Parent = deCoupledXYZ[minMidMaxXYZ[2].Key];
                exMiddleRightNode.IsLeftChild = true;
            }


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\tПосле ребалансировки узлов: x = {0}, y = {1}, z = {2}\n", x.NodeValue, y.NodeValue, z.NodeValue);
            Console.ForegroundColor = ConsoleColor.Gray;

            

            ColorNotification(new List<Node<T>> { x, y, z }, "Узлы ПОСЛЕ их ребалансировки");


            //VisitWidth();
        }

        // Метод, "обнуляющий" связи между узлами x, y, z - для метода Rebalance(...)
        Node<T>[] DeCoupled(Node<T> x, Node<T> y, Node<T> z)
        {
            Node<T>[] deCoupledXYZ = new Node<T>[3];

            // связь между узлами x - y
            if (y.IsLeftChild == true)
            {
                x.LeftChild = null;
                y.Parent = null;
            }
            else if (y.IsLeftChild == false)
            {
                x.RightChild = null;
                y.Parent = null;
            }

            // связь между узлами y - z

            if (z.IsLeftChild == true)
            {
                y.LeftChild = null;
                z.Parent = null;
            }
            else if (z.IsLeftChild == false)
            {
                y.RightChild = null;
                z.Parent = null;
            }


            deCoupledXYZ[0] = x;
            deCoupledXYZ[1] = y;
            deCoupledXYZ[2] = z;

            return deCoupledXYZ;
        }


        // Балансировка дерева (балансировка всех несбалансированных узлов дерева)
        public override void Balance()
        {
            Title("Балансировка дерева. Метод Balance()", ConsoleColor.Magenta, true);

            bool needBalance = false;

            Queue<Node<T>> nonballanced = AllNonBalanced(head);

            ColorNotification(ConvertToList(nonballanced), "Несбалансированные узлы (макс. глубина левого и правого поддерева узла отличается больше, чем на 1)");
            
            while (nonballanced.Count != 0)
            {
                needBalance = true;

                Node<T> x = nonballanced.Dequeue();
                Node<T> y = null;
                Node<T> z = null;

                int leftProfound = 0;
                int rightProfound = 0;

                IsEqualProfound(x, out leftProfound, out rightProfound);

                if (leftProfound > rightProfound)
                {
                    y = x.LeftChild;
                }
                else
                {
                    y = x.RightChild;
                }

                IsEqualProfound(y, out leftProfound, out rightProfound);

                if (leftProfound > rightProfound)
                {
                    z = y.LeftChild;
                }
                else
                {
                    z = y.RightChild;
                }


                Rebalance(x, y, z);

                nonballanced = AllNonBalanced(head);
            }

            if (needBalance)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n\n\tДерево теперь сбалансировано. Оно соответсвует АВЛ-дереву после всех балансировок узлов, выполненных выше:");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n\n\tДерево не нуждается в балансировке: все узлы сбалансированы. Соответсвует АВЛ-дереву\n");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
                        

            ShowTree();
        }

        // Преобразование типа  Queue<Node<T>> в тип List<Node<T>> (для метода Balance() )
        List<Node<T>> ConvertToList(Queue<Node<T>> queue)
        {
            List<Node<T>> result = new List<Node<T>>();

            foreach (var item in queue)
            {
                result.Add(item);
            }

            return result;
        }

        // 3. Удаление узла (3 случая, в том числе, когда узел имеет левого и правого потомков)
        public override void Delete(T node) //  удаляется первый узел из списка найденых
        {
            Title("Удаление узла " + node + ". Метод Delete(T node)", ConsoleColor.Magenta, true);

            List<Node<T>> nodesToDelete = FindNode(node);
            Node<T> deletedNode = null;

            ColorNotification(nodesToDelete, "Узел, который будет удален");


            if (nodesToDelete.Count > 0)
            {
                deletedNode = nodesToDelete.ToArray()[0];

                // если есть несколько узлов с одинаковыми значениями, то удаляется первый из найденных

                if (nodesToDelete.Count > 1)
                {
                    string message = "Есть несколько узлов с одинаковыми значениями " +  node.ToString() + 
                                     ". Удалится найденный первым среди них";

                    Message(deletedNode, deletedNode.IsLeftChild, message);
                }
            }

            

            if (deletedNode != null)
            {
                // 1-й случай: удаляемый узел не имеет ни одного потомка
                if (!HasChildren(deletedNode))
                {
                    Console.WriteLine("\n\t 1-й случай: удаляемый узел {0} не имеет ни одного потомка\n", node);

                    if (deletedNode.IsLeftChild == true)
                    {
                        deletedNode.Parent.LeftChild = null;
                    }
                    else
                    {
                        deletedNode.Parent.RightChild = null;
                    }

                    deletedNode = null;
                }
                // 2-й случай: удаляемый узел имеет только одного потомка
                else if (deletedNode.LeftChild != null & deletedNode.RightChild == null)
                {
                    Console.WriteLine("\n\t 2-й случай: удаляемый узел {0} имеет только одного потомка\n", node);

                    if (deletedNode.LeftChild.NodeValue.CompareTo(deletedNode.Parent.NodeValue) == -1)
                    {
                        deletedNode.Parent.LeftChild = deletedNode.LeftChild;
                        deletedNode.LeftChild.IsLeftChild = true;
                        deletedNode.LeftChild.Parent = deletedNode.Parent;
                    }
                    else
                    {
                        deletedNode.Parent.RightChild = deletedNode.LeftChild;
                        deletedNode.LeftChild.IsLeftChild = false;
                        deletedNode.LeftChild.Parent = deletedNode.Parent;
                    }

                }
                else if (deletedNode.LeftChild == null & deletedNode.RightChild != null)
                {
                    Console.WriteLine("\n\t 2-й случай: удаляемый узел {0} имеет только одного потомка\n", node);

                    if (deletedNode.RightChild.NodeValue.CompareTo(deletedNode.Parent.NodeValue) == -1)
                    {
                        deletedNode.Parent.LeftChild = deletedNode.RightChild;
                        deletedNode.RightChild.IsLeftChild = true;
                        deletedNode.RightChild.Parent = deletedNode.Parent;
                    }
                    else
                    {
                        deletedNode.Parent.RightChild = deletedNode.RightChild;
                        deletedNode.RightChild.IsLeftChild = false;
                        deletedNode.RightChild.Parent = deletedNode.Parent;
                    }
                }
                // 3-й случай: удаляемый узел имеет левого и правого потомка
                else if (deletedNode.LeftChild != null & deletedNode.RightChild != null)
                {
                    Console.WriteLine("\n\t 3-й случай: удаляемый узел {0} имеет левого и правого потомка\n", node);

                    Node<T> successor = Successor(deletedNode);
                    successor.IsLeftChild = deletedNode.IsLeftChild;


                    // successor не является потомком deletedNode 
                    if (deletedNode.LeftChild != successor & deletedNode.RightChild != successor)
                    {
                        // поставить правого потомка successor'а на место successor'а 

                        Node<T> insteadSuccessor = successor.RightChild;

                        if (insteadSuccessor != null)
                        {
                            insteadSuccessor.Parent = successor.Parent;
                        }

                        successor.Parent.LeftChild = insteadSuccessor;

                        successor.LeftChild = deletedNode.LeftChild;
                        successor.RightChild = deletedNode.RightChild;
                        successor.Parent = deletedNode.Parent;


                        if (deletedNode.IsLeftChild == true)
                        {
                            deletedNode.Parent.LeftChild = successor;
                            deletedNode.LeftChild.Parent = successor;
                            deletedNode.RightChild.Parent = successor;

                            deletedNode = successor;
                        }
                        else if (deletedNode.IsLeftChild == false)
                        {
                            deletedNode.Parent.RightChild = successor;
                            deletedNode.LeftChild.Parent = successor;
                            deletedNode.RightChild.Parent = successor;

                            deletedNode = successor;
                        }
                        else if (deletedNode.IsLeftChild == null)
                        {
                            successor.Parent = deletedNode.Parent;
                            deletedNode.LeftChild.Parent = successor;
                            deletedNode.RightChild.Parent = successor;

                            head = successor;
                        }

                    }
                    else // successor является потомком deletedNode
                    {

                        successor.LeftChild = deletedNode.LeftChild;
                        deletedNode.LeftChild.Parent = successor;

                        successor.Parent = deletedNode.Parent;


                        if (deletedNode.IsLeftChild == true)
                        {
                            deletedNode.Parent.LeftChild = successor;

                            deletedNode = successor;
                        }
                        else if (deletedNode.IsLeftChild == false)
                        {
                            deletedNode.Parent.RightChild = successor;

                            deletedNode = successor;
                        }
                        else if (deletedNode.IsLeftChild == null)
                        {
                            successor.Parent = deletedNode.Parent;
                            deletedNode.LeftChild.Parent = successor;
                            deletedNode.RightChild.Parent = successor;

                            head = successor;
                        }

                    }

                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\tУзел {0} удалён! \n", node);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n\tДерево после удаления узла {0}:\n", node);
                Console.ForegroundColor = ConsoleColor.Gray;

                ShowTree();
            }
        }


        public override void Message<T>(Node<T> value, bool? is_left, string message)
        {            
            Console.WriteLine("\n\t" + message + "\n");
            

            if (value.Parent != null)
            {
                if (is_left == true)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Узел со значением {0}. Это левый потомок родительского узла {1}", value.NodeValue, value.Parent.NodeValue);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if (is_left == false)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Узел со значением {0}. Это правый потомок родительского узла {1}", value.NodeValue, value.Parent.NodeValue);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Узел со значением {0}. Это корень дерева!", value.NodeValue, value.Parent);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            Console.WriteLine(new string('-', 30));
        }


        


        // Получение дерева в виде List<List<DisplayInfo>>, где содержатся узлы с null-значениями 
        List<List<DisplayInfo>> ListDisplay()
        {
            KeyValuePair<Node<T>, DisplayInfo> kvp = new KeyValuePair<Node<T>, DisplayInfo>();

            List<List<KeyValuePair<Node<T>, DisplayInfo>>> result = new List<List<KeyValuePair<Node<T>, DisplayInfo>>>();
            List<KeyValuePair<Node<T>, DisplayInfo>> lastResult = new List<KeyValuePair<Node<T>, DisplayInfo>>();
            List<KeyValuePair<Node<T>, DisplayInfo>> childrenLast = new List<KeyValuePair<Node<T>, DisplayInfo>>();

            DisplayInfo displ = null;

            current = head;


            int currenLevel = 0, orderNumber = 0, posit = 0, b = 0;
            int? parentIndex = null;


            string nodeValue;

            bool? isLeftChild = null;

            if (current != null)
            {
                displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, current.NodeValue.ToString(), isLeftChild);
                kvp = new KeyValuePair<Node<T>, DisplayInfo>(current, displ);
                childrenLast.Add(kvp);

                result.Add(childrenLast);
            }
            else
            {
                displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, nullValue, isLeftChild);
                kvp = new KeyValuePair<Node<T>, DisplayInfo>(current, displ);
                childrenLast.Add(kvp);

                result.Add(childrenLast);
            }



            bool isAllNull = false;

            while (isAllNull == false)
            {
                lastResult = result[result.Count - 1];

                foreach (var item in lastResult)
                {
                    isAllNull = true;

                    if (item.Value.NodeValue != nullValue)
                    {
                        isAllNull = false;
                        break;
                    }
                }

                if (isAllNull == true)
                {
                    break;
                }

                childrenLast = new List<KeyValuePair<Node<T>, DisplayInfo>>();

                orderNumber = 0;
                currenLevel++;

                for (int orderIndex = 0; orderIndex < lastResult.Count; orderIndex++)
                {
                    parentIndex = result[result.Count - 1][orderIndex].Value.OrderNumber;

                    isLeftChild = true;

                    // левый потомок
                    if (result[result.Count - 1][orderIndex].Key != null && result[result.Count - 1][orderIndex].Key.LeftChild == null)
                    {
                        nodeValue = nullValue;

                        displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, nodeValue, isLeftChild);
                        kvp = new KeyValuePair<Node<T>, DisplayInfo>(default(Node<T>), displ);

                        childrenLast.Add(kvp);
                    }
                    else if (result[result.Count - 1][orderIndex].Key != null)
                    {
                        Node<T> leftChild = result[result.Count - 1][orderIndex].Key.LeftChild;

                        nodeValue = leftChild.NodeValue.ToString();

                        displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, nodeValue, isLeftChild);
                        kvp = new KeyValuePair<Node<T>, DisplayInfo>(leftChild, displ);

                        childrenLast.Add(kvp);
                    }
                    else if (result[result.Count - 1][orderIndex].Key == null)
                    {
                        nodeValue = nullValue;

                        displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, nodeValue, isLeftChild);
                        kvp = new KeyValuePair<Node<T>, DisplayInfo>(default(Node<T>), displ);

                        childrenLast.Add(kvp);
                    }


                    orderNumber++;
                    isLeftChild = false;


                    // правый потомок
                    if (result[result.Count - 1][orderIndex].Key != null && result[result.Count - 1][orderIndex].Key.RightChild == null)
                    {
                        nodeValue = nullValue;

                        displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, nodeValue, isLeftChild);
                        kvp = new KeyValuePair<Node<T>, DisplayInfo>(default(Node<T>), displ);

                        childrenLast.Add(kvp);
                    }
                    else if (result[result.Count - 1][orderIndex].Key != null)
                    {
                        Node<T> rightChild = result[result.Count - 1][orderIndex].Key.RightChild;

                        nodeValue = rightChild.NodeValue.ToString();

                        displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, nodeValue, isLeftChild);
                        kvp = new KeyValuePair<Node<T>, DisplayInfo>(rightChild, displ);

                        childrenLast.Add(kvp);
                    }
                    else if (result[result.Count - 1][orderIndex].Key == null)
                    {
                        nodeValue = nullValue;

                        displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, nodeValue, isLeftChild);
                        kvp = new KeyValuePair<Node<T>, DisplayInfo>(default(Node<T>), displ);

                        childrenLast.Add(kvp);
                    }


                    orderNumber++;
                }


                result.Add(childrenLast);

            }




            // получение типа List<List<DisplayInfo>> из типа
            // List<List<KeyValuePair<Node<T>, DisplayInfo>>> 

            List<List<DisplayInfo>> listDisplay = new List<List<DisplayInfo>>();
            List<DisplayInfo> listInfo = new List<DisplayInfo>();

            int counter = 0;

            foreach (var item in result)
            {
                if (counter < result.Count - 1)
                {
                    listInfo = new List<DisplayInfo>();

                    foreach (var innerIter in item)
                    {
                        KeyValuePair<Node<T>, DisplayInfo> kvpDispl = innerIter;
                        // установить корректное b (ранее неизвестна глубина дерева)
                        kvpDispl.Value.B = (int)Math.Pow(2, result.Count - 1 - counter);

                        listInfo.Add(kvpDispl.Value);
                    }

                    listDisplay.Add(listInfo);

                }

                counter++;
            }


            //Удалить последний элемент, в котором только null - значения

            bool allNull = true;

            if (listDisplay.Count > 0)
            {
                foreach (var item in listDisplay[listDisplay.Count - 1])
                {

                    if (item.NodeValue != nullValue)
                    {
                        allNull = false;
                        break;
                    }
                }

                if (allNull == true)
                {
                    listDisplay.RemoveAt(listDisplay.Count - 1);
                }


                AllShifts(ref listDisplay);
            }
            


            return listDisplay;
        }


        // Поиск в списке типа List<List<DisplayInfo>> узлов, хранящихся в List<Node<T>> list
        // Возвращается список массивов List<int[]>. В каждом элементе массива int[] хранится
        // текущий уровень currenLevel и порядковый номер orderNumber найденного узла, 
        List<int[]> FindListDisplay(List<Node<T>> list)
        {
            List<int[]> listCoord = new List<int[]>();


            KeyValuePair<Node<T>, DisplayInfo> kvp = new KeyValuePair<Node<T>, DisplayInfo>();

            List<List<KeyValuePair<Node<T>, DisplayInfo>>> result = new List<List<KeyValuePair<Node<T>, DisplayInfo>>>();
            List<KeyValuePair<Node<T>, DisplayInfo>> lastResult = new List<KeyValuePair<Node<T>, DisplayInfo>>();
            List<KeyValuePair<Node<T>, DisplayInfo>> childrenLast = new List<KeyValuePair<Node<T>, DisplayInfo>>();

            DisplayInfo displ = null;

            current = head;


            int currenLevel = 0, orderNumber = 0, posit = 0, b = 0;
            int? parentIndex = null;



            string nodeValue;

            bool? isLeftChild = null;

            if (current != null)
            {
                displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, current.NodeValue.ToString(), isLeftChild);
                kvp = new KeyValuePair<Node<T>, DisplayInfo>(current, displ);
                childrenLast.Add(kvp);

                // добавить координату узла (currenLevel, orderNumber)
                AddCoord(list, current, ref listCoord, currenLevel, orderNumber);

                result.Add(childrenLast);
            }
            else
            {
                displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, nullValue, isLeftChild);
                kvp = new KeyValuePair<Node<T>, DisplayInfo>(current, displ);
                childrenLast.Add(kvp);

                result.Add(childrenLast);
            }



            bool isAllNull = false;

            while (isAllNull == false)
            {
                lastResult = result[result.Count - 1];

                foreach (var item in lastResult)
                {
                    isAllNull = true;

                    if (item.Value.NodeValue != nullValue)
                    {
                        isAllNull = false;
                        break;
                    }
                }

                if (isAllNull == true)
                {
                    break;
                }

                childrenLast = new List<KeyValuePair<Node<T>, DisplayInfo>>();

                orderNumber = 0;
                currenLevel++;

                for (int orderIndex = 0; orderIndex < lastResult.Count; orderIndex++)
                {
                    parentIndex = result[result.Count - 1][orderIndex].Value.OrderNumber; // 

                    isLeftChild = true;

                    // левый потомок
                    if (result[result.Count - 1][orderIndex].Key != null && result[result.Count - 1][orderIndex].Key.LeftChild == null)
                    {
                        nodeValue = nullValue;

                        displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, nodeValue, isLeftChild);
                        kvp = new KeyValuePair<Node<T>, DisplayInfo>(default(Node<T>), displ);

                        childrenLast.Add(kvp);
                    }
                    else if (result[result.Count - 1][orderIndex].Key != null)
                    {
                        Node<T> leftChild = result[result.Count - 1][orderIndex].Key.LeftChild;

                        nodeValue = leftChild.NodeValue.ToString();

                        displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, nodeValue, isLeftChild);
                        kvp = new KeyValuePair<Node<T>, DisplayInfo>(leftChild, displ);

                        // добавить координату узла (currenLevel, orderNumber)
                        AddCoord(list, leftChild, ref listCoord, currenLevel, orderNumber);

                        childrenLast.Add(kvp);
                    }
                    else if (result[result.Count - 1][orderIndex].Key == null)
                    {
                        nodeValue = nullValue;

                        displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, nodeValue, isLeftChild);
                        kvp = new KeyValuePair<Node<T>, DisplayInfo>(default(Node<T>), displ);

                        childrenLast.Add(kvp);
                    }


                    orderNumber++;
                    isLeftChild = false;


                    // правый потомок
                    if (result[result.Count - 1][orderIndex].Key != null && result[result.Count - 1][orderIndex].Key.RightChild == null)
                    {
                        nodeValue = nullValue;

                        displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, nodeValue, isLeftChild);
                        kvp = new KeyValuePair<Node<T>, DisplayInfo>(default(Node<T>), displ);

                        childrenLast.Add(kvp);
                    }
                    else if (result[result.Count - 1][orderIndex].Key != null)
                    {
                        Node<T> rightChild = result[result.Count - 1][orderIndex].Key.RightChild;

                        nodeValue = rightChild.NodeValue.ToString();

                        displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, nodeValue, isLeftChild);
                        kvp = new KeyValuePair<Node<T>, DisplayInfo>(rightChild, displ);

                        // добавить координату узла (currenLevel, orderNumber)
                        AddCoord(list, rightChild, ref listCoord, currenLevel, orderNumber);

                        childrenLast.Add(kvp);
                    }
                    else if (result[result.Count - 1][orderIndex].Key == null)
                    {
                        nodeValue = nullValue;

                        displ = new DisplayInfo(currenLevel, orderNumber, posit, parentIndex, b, nodeValue, isLeftChild);
                        kvp = new KeyValuePair<Node<T>, DisplayInfo>(default(Node<T>), displ);

                        childrenLast.Add(kvp);
                    }


                    orderNumber++;
                }


                result.Add(childrenLast);

            }


            return listCoord;
        }


        // Есть ли среди списка List<Node<T>> list узел, чья ссылка равна Node<T>
        bool IsInList(List<Node<T>> list, Node<T> checkNode)
        {
            bool result = false;

            foreach (Node<T> iter in list)
            {
                if (iter.Equals(checkNode))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }


        // Добавить в список - координаты узла (currentLevel и orderIndex), чья ссылка равна Node<T>
        void AddCoord(List<Node<T>> list, Node<T> checkNode, ref List<int[]> listCoord, int currentLevel, int orderIndex)
        {
            if (IsInList(list, checkNode))
            {
                int[] coord = new int[2];
                coord[0] = currentLevel;
                coord[1] = orderIndex;

                listCoord.Add(coord);
            }
        }


        //(Для отображения дерева)
        // Есть ли узел со значением (Posititon + длинна значения узла) > value
        // для сообщения о некорректном отображении дерева в виде графа
        bool IsExceed(List<List<DisplayInfo>> displayInfo, int value)
        {
            bool exceed = false;

            if (displayInfo.Count > 2)
            {

                foreach (var iterRow in displayInfo)
                {
                    foreach (var iterElem in iterRow)
                    {
                        if (iterElem.Position + iterElem.NodeValue.Length > value)
                        {
                            exceed = true;
                            break;
                        }
                    }
                }

            }

            if (exceed)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\nДерево в виде графа может отображаться в консоле некорректно!");
                Console.WriteLine("Это обусловленно максимальным числом символов, отображаемых в строке консоли, равным {0}\n\n", Console.LargestWindowWidth);
                Console.ForegroundColor = ConsoleColor.Gray;
            }


            return exceed;
        }

        //Вывод дерева с учетом длинны символов каждого узла
        public void ShowTree(params ColoredNodes[] coord)
        {
            Console.WriteLine("\n");

            Title("Изображение двоичного дерева в виде графа: ",ConsoleColor.White, false);


            List<List<DisplayInfo>> displayList = ListDisplay();

            IsExceed(displayList, Console.LargestWindowWidth);

            for (int curLevel = 0; curLevel < displayList.Count; curLevel++)
            {
                int spaces = 0;

                VerticalLine(displayList, curLevel);

                for (int orderIndex = 0; orderIndex < displayList[curLevel].Count; orderIndex++)
                {
                    if (orderIndex == 0)
                    {
                        spaces = displayList[curLevel][orderIndex].Position;
                    }
                    else
                    {
                        spaces = displayList[curLevel][orderIndex].Position - displayList[curLevel][orderIndex - 1].Position - displayList[curLevel][orderIndex - 1].NodeValue.Length;
                    }


                    Console.Write(new string(' ', spaces));

                    NodeColor(displayList[curLevel][orderIndex], coord);
                }

                Console.Write("\n");

                if (curLevel != displayList.Count - 2)
                {
                    HorizontalLine(displayList, curLevel);

                    Console.Write("\n");
                }

            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n  Условные обозначения:\n");
            Console.ForegroundColor = ConsoleColor.Gray;


            if (coord.Length > 0)
            {
                for (int i = 0; i < coord.Length; i++)
                {
                    Console.Write("  ");
                    Console.BackgroundColor = coord[0].Color;
                    Console.Write("  ");
                    Console.ResetColor();

                    Console.Write(" - {0}", coord[i].Notation);
                }

                //Console.WriteLine("\n");
            }


            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("  _____  Левый ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("потомок родителя\n");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("  _____  Правый ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("потомок родителя\n");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("    {0}\t Корень ", head.NodeValue);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("дерева\n\n\n\n");

            Console.WriteLine("\n");
        }


        // Задать цвет узла и связи с родителем 
        void NodeColor(DisplayInfo displayNode, params ColoredNodes[] coord)
        {
            ConsoleColor color = ConsoleColor.Gray;

            bool? isLeftChild = displayNode.IsLeftChild;

            if (isLeftChild == null)
            {
                color = ConsoleColor.Red;
            }
            else if (isLeftChild == true)
            {

                color = ConsoleColor.Yellow;
            }
            else if (isLeftChild == false)
            {

                color = ConsoleColor.Cyan;
            }


            if (coord.Length > 0)
            {
                for (int i = 0; i < coord.Length; i++)
                {
                    List<int[]> list = coord[i].BackGroundNodes;

                    int currentLevel = displayNode.CurrentLevel;
                    int orderNumber = displayNode.OrderNumber;


                    foreach (int[] item in list)
                    {
                        if (currentLevel == item[0] && orderNumber == item[1])
                        {
                            Console.BackgroundColor = coord[i].Color;
                            break;
                        }
                    }

                }

            }


            Console.ForegroundColor = color;
            Console.Write(displayNode.NodeValue);
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.ResetColor();
        }



        // Добавление перечня узлов, фон которых должен выделяться при отображении дерева в виде графа 
        void ColorNotification(List<Node<T>> foundNodes, string notation)
        {
            if (foundNodes.Count > 0)
            {
                List<int[]> listCoord = FindListDisplay(foundNodes);

                ColoredNodes coloredNodes = new ColoredNodes(listCoord, ConsoleColor.DarkGray, notation + "\n\n");

                List<List<DisplayInfo>> displayList = ListDisplay();

                ShowTree(coloredNodes);
            }
            else if (foundNodes.Count == 0)
            {
                ShowTree();
            }

        }


        // Заголовок метода, выделенный цветом
        void Title(string title, ConsoleColor color, bool lines)
        {
            Console.ForegroundColor = color;
            if (lines)
            {
                Console.WriteLine("\n");
                Console.WriteLine(new string('-', 80));
                Console.WriteLine("\n\t\t\t" + title + "\n");
                Console.WriteLine(new string('-', 80));
                Console.WriteLine("\n");
            }
            else
            {
                Console.WriteLine("\n\n\t\t\t" + title + "\n\n");
            }
            
            Console.ForegroundColor = ConsoleColor.Gray;
        }


        // Смещение только одного уровня, начиная с заданного узла 
        public void ShiftOneRow(ref List<List<DisplayInfo>> displayList, DisplayInfo startNode, int shift)
        {
            Console.WriteLine("\nСмещение узла на {0} ед. в уровне {1}. Начальный узел {2} \n", shift, startNode.CurrentLevel, startNode.NodeValue);


            int fromIndex = startNode.OrderNumber;
            int currentLevel = startNode.CurrentLevel;
            int endIndex = displayList[currentLevel].Count - 1;

            for (int indexList = fromIndex; indexList <= endIndex; indexList++)
            {
                displayList[currentLevel][indexList].Position += shift;
            }
        }

        // Смещение, начиная с заданного узла и всех их потомков
        public void ShiftRowChildren(ref List<List<DisplayInfo>> displayList, DisplayInfo startNode, int shift)
        {
            Console.WriteLine("\nСмещение узла на {0} ед. с уровня {1} и всех дочерних. Начальный узел {2} \n", shift, startNode.CurrentLevel, startNode.NodeValue);

            int indexLeftChild = 0;

            for (int curLevel = startNode.CurrentLevel; curLevel < displayList.Count - 1; curLevel++)
            {
                ShiftOneRow(ref displayList, startNode, shift);

                indexLeftChild = 2 * startNode.OrderNumber;

                startNode = displayList[curLevel + 1][indexLeftChild];
            }
        }


        // Нарисовать горизонтальные линии связи цвета, соответствующего цвету левого или правого потомка
        void HorizontalLine(List<List<DisplayInfo>> displayList, int currentLevel)
        {
            if (currentLevel < displayList.Count - 2)
            {
                int startIndex = 0, endIndex = 0;

                int spaces = 0;

                for (int orderIndex = 0; orderIndex < displayList[currentLevel].Count; orderIndex++)
                {
                    int indexLeftChild = 2 * orderIndex;
                    int indexRightChild = 2 * orderIndex + 1;

                    int lengthLeftChild = displayList[currentLevel + 1][indexLeftChild].NodeValue.Length;
                    int lengthRightChild = displayList[currentLevel + 1][indexRightChild].NodeValue.Length;


                    DisplayInfo currentNode = displayList[currentLevel][orderIndex];
                    DisplayInfo currentLeftChild = displayList[currentLevel + 1][indexLeftChild];
                    DisplayInfo currentRightChild = displayList[currentLevel + 1][indexRightChild];


                    // от предыдущего правого потомка (или от края) до начала позиции левого потомка (пробелы)

                    if (orderIndex > 0)
                    {
                        DisplayInfo PreviousRightChild = displayList[currentLevel + 1][indexLeftChild - 1];
                        startIndex = PreviousRightChild.Position + PreviousRightChild.NodeValue.Length;
                    }



                    endIndex = currentLeftChild.Position;

                    spaces = endIndex - startIndex;

                    Console.Write(new string(' ', spaces));


                    // от начала позиции левого потомка до конца позиции левого потомка (пробелы)

                    startIndex = currentLeftChild.Position;

                    endIndex = currentLeftChild.Position + currentLeftChild.NodeValue.Length;

                    spaces = endIndex - startIndex;

                    Console.Write(new string(' ', spaces));


                    // от конца позиции левого потомка до начала позиции родителя (_цветом)

                    startIndex = currentLeftChild.Position + currentLeftChild.NodeValue.Length;

                    endIndex = currentNode.Position;

                    spaces = endIndex - startIndex;

                    if (currentLeftChild.NodeValue != nullValue)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;

                        if (spaces == 1)
                        {
                            Console.Write(new string('/', spaces));
                        }
                        else if (spaces >= 2)
                        {
                            Console.Write(new string('_', spaces - 1) + new string('/', 1));
                        }


                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else
                    {
                        Console.Write(new string(' ', spaces));
                    }


                    // от начала позиции родителя до конца родителя (пробелы)

                    startIndex = currentNode.Position;

                    endIndex = currentNode.Position + currentNode.NodeValue.Length;

                    spaces = endIndex - startIndex;

                    Console.Write(new string(' ', spaces));



                    // от конца родителя до начала позиции правого потомка (_цветом)

                    startIndex = currentNode.Position + currentNode.NodeValue.Length;////

                    endIndex = currentRightChild.Position;

                    spaces = endIndex - startIndex;

                    if (currentRightChild.NodeValue != nullValue)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;

                        if (spaces == 1)
                        {
                            Console.Write(new string('\\', spaces));
                        }
                        else if (spaces >= 2)
                        {
                            Console.Write(new string('\\', 1) + new string('_', spaces - 1));
                        }

                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else
                    {
                        Console.Write(new string(' ', spaces));
                    }

                    // от начала позиции правого потомка до конца позиции правого потомка (пробелы)

                    startIndex = currentRightChild.Position;

                    endIndex = currentRightChild.Position + currentRightChild.NodeValue.Length;

                    spaces = endIndex - startIndex;

                    Console.Write(new string(' ', spaces));



                }


            }
        }


        // Нарисовать горизонтальные линии связи цвета, соответствующего цвету левого или правого потомка
        void VerticalLine(List<List<DisplayInfo>> displayList, int currentLevel)
        {
            if (currentLevel > 0)
            {
                int curPosit = 0;

                int counter = 0;

                for (int orderIndex = 0; orderIndex < displayList[currentLevel].Count; orderIndex++)
                {

                    if (orderIndex % 2 == 0)
                    {
                        curPosit = displayList[currentLevel][orderIndex].Position +
                                   displayList[currentLevel][orderIndex].NodeValue.Length;

                        while (counter < curPosit - 1)
                        {
                            Console.Write(" ");
                            counter++;
                        }


                        if (displayList[currentLevel][orderIndex].NodeValue != nullValue && orderIndex % 2 == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(new string('/', 1));
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else if (displayList[currentLevel][orderIndex].NodeValue == nullValue)
                        {
                            Console.Write(new string(' ', 1));
                        }
                    }



                    if (orderIndex % 2 != 0)
                    {
                        curPosit = displayList[currentLevel][orderIndex].Position;

                        while (counter < curPosit)
                        {
                            Console.Write(" ");
                            counter++;
                        }

                        if (displayList[currentLevel][orderIndex].NodeValue != nullValue && orderIndex % 2 != 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(new string('\\', 1));
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else if (displayList[currentLevel][orderIndex].NodeValue == nullValue)
                        {
                            Console.Write(new string(' ', 1));
                        }

                        if (orderIndex == displayList[currentLevel].Count - 1)
                        {
                            Console.Write("\n");
                        }
                    }

                    counter++;//
                }
            }

            //Console.Write("\n");
        }




        // Cдвиг узлов (для метода визуализации дерева)
        // Увеличить отступ узлов на заданную величину shift, начиная с узла startNode и отступы родительских узлов
        // для симметричного отображения дерева, независимо от количества символов в значении узла
        public void AllShifts(ref List<List<DisplayInfo>> displayList)
        {

            for (int curLevel = displayList.Count - 1; curLevel >= 0; curLevel--)
            {
                int posit = 0;
                int b = displayList[curLevel][0].B;


                for (int orderIndex = 0; orderIndex < displayList[curLevel].Count; orderIndex++)
                {

                    int indexParent = 0;

                    if (curLevel == displayList.Count - 1)
                    {
                        // от конца предыдущего узла (или от нуля) до начала текущего узла

                        if (displayList[curLevel][orderIndex].ParentIndex != null)
                        {
                            indexParent = (int)displayList[curLevel][orderIndex].ParentIndex;
                        }



                        if (orderIndex > 0)
                        {
                            if (orderIndex % 2 == 0)
                            {
                                posit = displayList[curLevel][orderIndex - 1].Position +
                                        displayList[curLevel][orderIndex - 1].NodeValue.Length + b;


                            }

                            if (orderIndex % 2 != 0)
                            {
                                if (displayList[curLevel][orderIndex].ParentIndex != null)
                                {
                                    posit = displayList[curLevel][orderIndex - 1].Position +
                                        displayList[curLevel][orderIndex - 1].NodeValue.Length +
                                        displayList[curLevel - 1][indexParent].NodeValue.Length;
                                }
                                else
                                {
                                    posit = displayList[curLevel][orderIndex - 1].Position +
                                        displayList[curLevel][orderIndex - 1].NodeValue.Length;
                                }



                            }
                        }


                        displayList[curLevel][orderIndex].Position = posit; //

                    }
                    else
                    {
                        // на предпоследнем уровене дерева позиция узла равна позиции конца позиции левого узла

                        if (curLevel == displayList.Count - 2)
                        {
                            int indLftChld = orderIndex * 2;

                            displayList[curLevel][orderIndex].Position = displayList[curLevel + 1][indLftChld].Position +
                                                                         displayList[curLevel + 1][indLftChld].NodeValue.Length;

                        }


                        int indexLeftChild = 2 * orderIndex;
                        int indexRightChild = 2 * orderIndex + 1;


                        int positEndLeft = displayList[curLevel + 1][indexLeftChild].Position +
                                           displayList[curLevel + 1][indexLeftChild].NodeValue.Length;

                        int positBeginRight = displayList[curLevel + 1][indexRightChild].Position;

                        int delta = positBeginRight - positEndLeft;

                        int curLength = displayList[curLevel][orderIndex].NodeValue.Length;



                        if (curLength == delta)
                        {
                            displayList[curLevel][orderIndex].Position = positEndLeft;
                        }
                        else if (curLength < delta)
                        {
                            displayList[curLevel][orderIndex].Position = ((positEndLeft + positBeginRight) - curLength) / 2;

                        }
                        else if (curLength > delta)
                        {
                            displayList[curLevel][orderIndex].Position = positEndLeft;

                            int shift = displayList[curLevel][orderIndex].NodeValue.Length - delta;

                            // сдвигаем правого потомка текущего узла и узлы, правее него и их потомков
                            ShiftRowChildren(ref displayList, displayList[curLevel + 1][indexRightChild], shift);
                        }

                        // если расстояние между концом левого потомка и началом правого потомка меньше, чем длинна родителя

                        if (orderIndex > 0)
                        {
                            int prevEnd = displayList[curLevel][orderIndex - 1].Position +
                                          displayList[curLevel][orderIndex - 1].NodeValue.Length;

                            // если расстояние между концом узла слева от текущего узла и началом текущего узла меньше b

                            if (prevEnd + b > displayList[curLevel][orderIndex].Position)
                            {
                                delta = displayList[curLevel][orderIndex].Position - prevEnd;

                                ShiftRowChildren(ref displayList, displayList[curLevel][orderIndex], b - delta);
                            }

                        }
                    }

                }
            }

        }



    }


    // Класс для храниения сведения об узле дерева
    class DisplayInfo
    {
        public DisplayInfo(int currentLevel, int orderNumber, int posit, int? parentIndex, int b, string nodeValue, bool? isLeftChild)
        {
            CurrentLevel = currentLevel; 
            OrderNumber = orderNumber; 
            Position = posit;
            ParentIndex = parentIndex;
            B = b;
            NodeValue = nodeValue;
            IsLeftChild = isLeftChild;
        }

        public int OrderNumber { get; set; } // порядковый номер узла, если бы узлы дерева не могли иметь значение null

        public int Position { get; set; } // позиция в строке, с которой записывается значение узла при визуализации деерва в виде графа

        public int CurrentLevel { get; set; } // текущий уровень узла в дереве        

        public int? ParentIndex { get; set; } // индекс родителя текущего узла

        public int B { get; set; } // расстояние между узлом с нечётным и чётным порядковым номером OrderNumber

        public string NodeValue { get; set; } // значение узла

        public bool? IsLeftChild { get; set; } // является ли текущий узел левым потомком своего родителя
    }


    // Класс для задания стиля отображения сведений об узле дерева
    class ColoredNodes
    {
        public ColoredNodes(List<int[]> backGroundNodes, ConsoleColor color, string notation)
        {
            BackGroundNodes = backGroundNodes;
            Color = color;
            Notation = notation;
        }

        public List<int[]> BackGroundNodes { get; set; } // в int[] вносятся currentLevel и orderIndex   
        public ConsoleColor Color { get; set; } // цвет фона узла
        public string Notation { get; set; } // текстовое описание, поясняющее условное обозначение цвета фона узла
    }


  



    class Program
    {
        static void Main(string[] args)
        {
            Console.BufferHeight = 2000;
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight); // не изменять 
            

            Tree<int> tree = new Tree<int>();

            

            tree.Add(20);

            tree.Add(10);
            tree.Add(5);

            tree.Add(25);

            tree.Add(35);
            tree.Add(21);

            tree.Add(18);
            tree.Add(19);

            tree.Add(31);
            tree.Add(34);            
            tree.Add(32);

            // Двоичное дерево:

            //                             20          
            //                              
            //               10                        25       
            //
            //          5          18              21          35
            // 
            //                         19                   31
            //
            //                                                 34
            //
            //                                             32


            tree.VisitWidth(); // Обход дерева в ширину

            tree.VisitDepth(); // Обход дерева в глубину

            tree.FindNode(10); // Найти узел с заданным значением
           
            tree.Delete(10);  // Удаление узла с заданным значением            

            tree.Balance(); // Балансировка дерева (перестроение узлов и приведение к АВЛ-дереву)


            

            

            Console.ReadKey();
        }
    }
}
