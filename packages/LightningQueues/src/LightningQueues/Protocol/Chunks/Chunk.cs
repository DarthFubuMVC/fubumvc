using System;
using System.IO;
using System.Threading.Tasks;
using LightningQueues.Logging;

namespace LightningQueues.Protocol.Chunks
{
    public abstract class Chunk
    {
        protected readonly ILogger _logger;
        protected readonly string _endpoint;

        protected Chunk(ILogger logger, string endpoint = null)
        {
            _logger = logger;
            _endpoint = endpoint ?? "unknown";
        }

        public async Task ProcessAsync(Stream stream)
        {
            try
            {
                _logger.Debug("{0} with Endpoint: {1}", ToString(), _endpoint);
                await ProcessInternalAsync(stream).ConfigureAwait(false);
                _logger.Debug("Successfully {0} with Endpoint: {1}", ToString(), _endpoint);
            }
            catch (ObjectDisposedException)
            {
                // This will be thrown if the operation timed out and the stream was disposed.
                throw;
            }
            catch (Exception exception)
            {
                _logger.Info("Could not process {0} for Endpoint: {1}", exception, ToString(), _endpoint);
                throw;
            }
        }

        protected abstract Task ProcessInternalAsync(Stream stream);
    }

    public abstract class Chunk<T> : Chunk
    {
        protected Chunk(ILogger logger, string endpoint) : base(logger, endpoint)
        {
        }

        public async Task<T> GetAsync(Stream stream)
        {
            T retVal = default(T);
            try
            {
                _logger.Debug("{0} with Endpoint: {1}", ToString(), _endpoint);
                retVal = await GetInternalAsync(stream).ConfigureAwait(false);
                _logger.Debug("Successfully {0} with Endpoint: {1}", ToString(), _endpoint);
            }
            catch (ObjectDisposedException)
            {
                // This will be thrown if the operation timed out and the stream was disposed.
                throw;
            }
            catch (Exception exception)
            {
                _logger.Info("Could not process {0} for Endpoint: {1}", exception, ToString(), _endpoint);
                throw;
            }
            return retVal;
        }

        protected abstract Task<T> GetInternalAsync(Stream stream);

        protected override Task ProcessInternalAsync(Stream stream)
        {
            return new Task(() => { });
        }
    }
}