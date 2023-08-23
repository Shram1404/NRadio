using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Collections;
using NRadio.Models;

namespace NRadio.Core.Helpers
{

    public class IncrementalPlaylist : IIncrementalSource<RadioStation>
    {
        private const int Delay = 100;

        private readonly List<RadioStation> playlist;

        public IncrementalPlaylist(List<RadioStation> playlist)
        {
            this.playlist = playlist;
            System.Diagnostics.Debug.WriteLine("StationsPlaylist created");
        }

        public async Task<IEnumerable<RadioStation>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var result = (from p in playlist
                          select p).Skip(pageIndex * pageSize).Take(pageSize);

            await Task.Delay(Delay, cancellationToken);
            return result;
        }
    }
}
