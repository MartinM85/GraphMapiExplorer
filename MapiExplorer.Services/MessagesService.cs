using MapiExplorer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Me.Messages.Item;
using Microsoft.Graph.Me.SendMail;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace MapiExplorer.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly GraphServiceClient _client;
        private readonly ILogger _logger;
        public MessagesService(GraphServiceClient client, ILogger<MessagesService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<Message>> GetMessagesAsync(string subject, string senderMail, DateTime? start, DateTime? end)
        {
            try
            {
                var messagesResponse = await _client.Me.Messages.GetAsync(x =>
                {
                    x.QueryParameters.Select = new string[] { "id", "sender", "subject", "receivedDateTime", "bodyPreview" };
                    x.QueryParameters.Top = 50;
                    var filter = string.Empty;
                    if (!string.IsNullOrEmpty(subject))
                    {
                        filter += $"contains(subject, '{subject}')";
                    }
                    if (!string.IsNullOrEmpty(senderMail))
                    {
                        var filterSender = $"sender/emailAddress/address eq '{senderMail}'";
                        filter = filter.Length > 0
                                    ? $"({filter} or {filterSender})"
                                    : filterSender;
                    }
                    if (start.HasValue)
                    {
                        var filterStart = $"receivedDateTime ge {start.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")}";
                        filter = filter.Length > 0
                                    ? $"{filter} and {filterStart}"
                                    : filterStart;
                    }
                    if (end.HasValue)
                    {
                        var filterEnd = $"receivedDateTime le {end.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")}";
                        filter = filter.Length > 0
                                    ? $"{filter} and {filterEnd}"
                                    : filterEnd;
                    }
                    if (!string.IsNullOrEmpty(filter))
                    {
                        x.QueryParameters.Filter = filter;
                    }
                });
                return messagesResponse.Value;
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to load messages.");
                return new List<Message>();
            }
        }

        public async Task<bool> SendToMailAsync(SendMessageDto message)
        {
            var body = new SendMailPostRequestBody
            {
                Message = new Message
                {
                    Subject = message.Subject,
                    Sender = new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = message.Sender
                        }
                    },
                    ToRecipients = message.ToRecipients.Select(x =>
                        new Recipient
                        {
                            EmailAddress = new EmailAddress { Address = x }
                        }
                    ).ToList(),
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = message.Content
                    }
                },
                SaveToSentItems = true
            };
            try
            {
                await _client.Me.SendMail.PostAsync(body);
                return true;
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to send mail.");
                return false;
            }
        }

        public async Task<SingleValueLegacyExtendedProperty> GetSingleValueExtendedProperties(string messageId, string propertyId)
        {
            var request = _client.Me.Messages[messageId].ToGetRequestInformation(x =>
            {
                x.QueryParameters.Select = new[] { "id" };
            });
            // hack => expand not supported
            request.QueryParameters["%24expand"] = new string[] { $"singleValueExtendedProperties($filter=id eq '{propertyId}')" };
            var index = request.UrlTemplate.Length;
            request.UrlTemplate = request.UrlTemplate.Insert(index - 1, ",%24expand");

            try
            {
                var result = await new MessageItemRequestBuilder(request.URI.OriginalString, _client.RequestAdapter).GetAsync();
                return result.SingleValueExtendedProperties?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, $"Failed to read value of extended property '{propertyId}'.");
                return null;
            }
        }

        public async Task<bool> SetSingleValueExtendedPropertyValueAsync(string messageId, string propertyId, string propertyValue)
        {
            var body = new Message
            {
                SingleValueExtendedProperties = new List<SingleValueLegacyExtendedProperty>
                {
                    new SingleValueLegacyExtendedProperty
                    {
                        Id = propertyId,
                        Value = propertyValue
                    }
                }
            };
            try
            {
                await _client.Me.Messages[messageId].PatchAsync(body);
                return true;
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to set value of single extended property.");
                return false;
            }
        }

        public async Task<MultiValueLegacyExtendedProperty> GetMultiValueExtendedProperties(string messageId, string propertyId)
        {
            var request = _client.Me.Messages[messageId].ToGetRequestInformation(x =>
            {
                x.QueryParameters.Select = new[] { "id" };
            });
            request.QueryParameters["%24expand"] = new string[] { $"multiValueExtendedProperties($filter=id eq '{propertyId}')" };
            var index = request.UrlTemplate.Length;
            request.UrlTemplate = request.UrlTemplate.Insert(index - 1, ",%24expand");
            try
            {
                var result = await new MessageItemRequestBuilder(request.URI.OriginalString, _client.RequestAdapter).GetAsync();
                return result.MultiValueExtendedProperties?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, $"Failed to read value of extended property '{propertyId}'.");
                return null;
            }
        }

        public async Task<bool> SetMultiValueExtendedPropertyValueAsync(string messageId, string propertyId, List<string> propertyValues)
        {
            var body = new Message
            {
                MultiValueExtendedProperties = new List<MultiValueLegacyExtendedProperty>
                {
                    new MultiValueLegacyExtendedProperty
                    {
                        Id = propertyId,
                        Value = propertyValues
                    }
                }
            };
            try
            {
                await _client.Me.Messages[messageId].PatchAsync(body);
                return true;
            }
            catch (Exception ex)
            {
                if (ex is ODataError oDataError)
                {
                    ODataErrorHelper.LogODataError(_logger, oDataError);
                }
                _logger.LogError(ex, "Failed to set value of multi value extended property.");
                return false;
            }
        }
    }
}
