﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using Nest.Resolvers;
using Newtonsoft.Json;


namespace Nest
{
	public class ConnectionStatus
	{
		private string mockJsonResponse;
		private static readonly string _printFormat;
		private static readonly string _errorFormat;
		public bool Success { get; private set; }
		public ConnectionError Error { get; private set; }
		public string RequestMethod { get; internal set; }
		public string RequestUrl { get; internal set; }

		private string _result;
		public string Result
		{
			get { return _result ?? (_result = this.ResultBytes.Utf8String()); }
		}

		public byte[] ResultBytes { get; internal set; }

		private static readonly byte _startAccolade = (byte)'{';
		private ElasticsearchResponse _response;
		public ElasticsearchResponse Response
		{
			get
			{
				if (ResultBytes == null || ResultBytes.Length == 0)
					return null;
				if (ResultBytes[0] != _startAccolade)
					return null;

				if (_response == null)
					this._response = this.Deserialize<ElasticsearchResponse>();
				return this._response;
			}
		}

		public string Request { get; internal set; }
		public NestSerializer Serializer { get; private set; }
		public ElasticInferrer Infer { get; private set; }

		//TODO probably nicer if we make this factory ConnectionStatus.Error() and ConnectionStatus.Valid()
		//and make these constructors private.
		private ConnectionStatus(IConnectionSettings settings)
		{
			this.Serializer = new NestSerializer(settings);
			this.Infer = new ElasticInferrer(settings);
		}

		public ConnectionStatus(IConnectionSettings settings, Exception e) : this(settings)
		{
			this.Success = false;
			this.Error = new ConnectionError(e);
			this.ResultBytes = this.Error.Response.Utf8Bytes();
		}
		public ConnectionStatus(IConnectionSettings settings, string result) : this(settings)
		{
			this.Success = true;
			this.ResultBytes = Encoding.UTF8.GetBytes(result);
		}
		public ConnectionStatus(IConnectionSettings settings, byte[] result) : this(settings)
		{
			this.Success = true;
			this.ResultBytes = result;
		}
		static ConnectionStatus()
		{
			_printFormat = "StatusCode: {1}, {0}\tMethod: {2}, {0}\tUrl: {3}, {0}\tRequest: {4}, {0}\tResponse: {5}";
			_errorFormat = "{0}\tExceptionMessage: {1}{0}\t StackTrace: {2}";
		}

		/// <summary>
		/// Returns a response of type R based on the connection status by trying parsing status.Result into R
		/// </summary>
		/// <returns></returns>
		public virtual T Deserialize<T>(bool allow404 = false) where T : class
		{
			if (typeof(BaseResponse).IsAssignableFrom(typeof(T)))
			  return this.Serializer.Deserialize<T>(this, notFoundIsValidResponse: allow404);
			return this.Serializer.Deserialize<T>(this.Result, notFoundIsValidResponse: allow404);
		}

		public override string ToString()
		{
			var r = this;
			var e = r.Error;
			var print = _printFormat.F(
			  Environment.NewLine,
			  e != null ? e.HttpStatusCode : HttpStatusCode.OK,
			  r.RequestMethod,
			  r.RequestUrl,
			  r.Request,
			  r.Result
			);
			if (!this.Success)
			{
				print += _errorFormat.F(Environment.NewLine, e.ExceptionMessage, e.OriginalException.StackTrace);
			}
			return print;
		}

	}
}
