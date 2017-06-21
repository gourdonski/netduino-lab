using System;
using Microsoft.SPOT;

namespace NetduinoLab.Web
{
    public enum HttpMethod
    {
        Null,
        Delete,
        Get,
        Head,
        Post,
        Put
    }

    public enum HttpResponseStatus
    {
        OK = 200,
        Accepted = 202,
        MovedPermanently = 301,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        InternalServerError = 500
    }

    public enum HttpContentType
    {
        Null,
        ApplicationJavascript,
        ApplicationJson,
        ApplicationPdf,
        AudioMpeg,
        Image,
        ImageGif,
        ImageJpeg,
        ImagePng,
        TextCss,
        TextHtml,
        TextJavascript,
        TextPlain,
        TextXml,
        VideoMpeg
    }
}
