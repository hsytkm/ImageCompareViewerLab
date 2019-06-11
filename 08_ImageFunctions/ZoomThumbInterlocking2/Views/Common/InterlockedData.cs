using System.Threading;

namespace ZoomThumb.Views.Common
{
    class UniqueId
    {
        private static int BaseId = 0;  // 1からID割り振られる
        public int Id { get; }

        // newする度に一意IDをインクリ
        public UniqueId() => Id = Interlocked.Increment(ref BaseId);
    }

    struct InterlockedData<T>
    {
        public int PublisherId { get; }
        public T Data { get; }
        public InterlockedData(int id, T data)
        {
            PublisherId = id;
            Data = data;
        }
    }

}
