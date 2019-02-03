using System;
using System.Collections.Generic;
using System.Linq;

namespace ThosoImage.Extensions
{
    public static class IListExtension
    {
#if false
        /// <summary>
        /// 要素を昇順方向に回転させる
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">対象リスト</param>
        public static IList<T> RotateAscendingOrder<T>(this IList<T> list)
        {
            if (list is null) throw new ArgumentNullException();

            var tail = list.Last<T>();
            for (int i = 0; i < list.Count - 1; i++)
            {
                list[i + 1] = list[i];
            }
            list[0] = tail;
            return list;
        }

        /// <summary>
        /// 要素を降順方向に回転させる
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">対象リスト</param>
        public static IList<T> RotateDescendingOrder<T>(this IList<T> list)
        {
            if (list is null) throw new ArgumentNullException();

            var head = list.First<T>();
            for (int i = list.Count - 1; i > 0; i--)
            {
                list[i] = list[i - 1];
            }
            list[list.Count - 1] = head;
            return list;
        }

        /// <summary>
        /// 要素を昇順方向に指定回数だけ回転させる
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">対象リスト</param>
        /// <param name="loop">回転数</param>
        public static IList<T> RotateAscendingOrder<T>(this IList<T> list, int loop)
        {
            if (loop == 0) return list;

            // ◆もう少しマシな実装があると思う…
            if (loop > 0)
            {
                for (int i = 0; i < loop; i++) RotateAscendingOrder(list);
            }
            else
            {
                for (int i = 0; i < -loop; i++) RotateDescendingOrder(list);
            }
            return list;
        }

        /// <summary>
        /// 要素を降順方向に指定回数だけ回転させる
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">対象リスト</param>
        /// <param name="loop">回転数</param>
        public static IList<T> RotateDescendingOrder<T>(this IList<T> list, int loop)
        {
            if (loop == 0) return list;

            // ◆もう少しマシな実装があると思う…
            if (loop > 0)
            {
                for (int i = 0; i < loop; i++) RotateDescendingOrder(list);
            }
            else
            {
                for (int i = 0; i < -loop; i++) RotateAscendingOrder(list);
            }
            return list;
        }
#endif
    }
}
