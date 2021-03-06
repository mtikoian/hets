/*
 * REST API Documentation for the MOTI Hired Equipment Tracking System (HETS) Application
 *
 * The Hired Equipment Program is for owners/operators who have a dump truck, bulldozer, backhoe or  other piece of equipment they want to hire out to the transportation ministry for day labour and  emergency projects.  The Hired Equipment Program distributes available work to local equipment owners. The program is  based on seniority and is designed to deliver work to registered users fairly and efficiently  through the development of local area call-out lists. 
 *
 * OpenAPI spec version: v1
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.SwaggerGen.Annotations;
using HETSAPI.Models;
using HETSAPI.ViewModels;
using HETSAPI.Services;
using HETSAPI.Authorization;

namespace HETSAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AttachmentController : Controller
    {
        private readonly IAttachmentService _service;

        /// <summary>
        /// Create a controller and set the service
        /// </summary>
        public AttachmentController(IAttachmentService service)
        {
            _service = service;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <response code="201">Attachment created</response>
        [HttpPost]
        [Route("/api/attachments/bulk")]
        [SwaggerOperation("AttachmentsBulkPost")]
        [RequiresPermission(Permission.ADMIN)]
        public virtual IActionResult AttachmentsBulkPost([FromBody]Attachment[] items)
        {
            return this._service.AttachmentsBulkPostAsync(items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <response code="200">OK</response>
        [HttpGet]
        [Route("/api/attachments")]
        [SwaggerOperation("AttachmentsGet")]
        [SwaggerResponse(200, type: typeof(List<Attachment>))]
        public virtual IActionResult AttachmentsGet()
        {
            return this._service.AttachmentsGetAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id of Attachment to delete</param>
        /// <response code="200">OK</response>
        /// <response code="404">Attachment not found</response>
        [HttpPost]
        [Route("/api/attachments/{id}/delete")]
        [SwaggerOperation("AttachmentsIdDeletePost")]
        public virtual IActionResult AttachmentsIdDeletePost([FromRoute]int id)
        {
            return this._service.AttachmentsIdDeletePostAsync(id);
        }

        /// <summary>
        /// Returns the binary file component of an attachment
        /// </summary>
        /// <param name="id">Attachment Id</param>
        /// <response code="200">OK</response>
        /// <response code="404">Attachment not found in system</response>
        [HttpGet]
        [Route("/api/attachments/{id}/download")]
        [SwaggerOperation("AttachmentsIdDownloadGet")]
        public virtual IActionResult AttachmentsIdDownloadGet([FromRoute]int id)
        {
            return this._service.AttachmentsIdDownloadGetAsync(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id of Attachment to fetch</param>
        /// <response code="200">OK</response>
        /// <response code="404">Attachment not found</response>
        [HttpGet]
        [Route("/api/attachments/{id}")]
        [SwaggerOperation("AttachmentsIdGet")]
        [SwaggerResponse(200, type: typeof(Attachment))]
        public virtual IActionResult AttachmentsIdGet([FromRoute]int id)
        {
            return this._service.AttachmentsIdGetAsync(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id of Attachment to fetch</param>
        /// <param name="item"></param>
        /// <response code="200">OK</response>
        /// <response code="404">Attachment not found</response>
        [HttpPut]
        [Route("/api/attachments/{id}")]
        [SwaggerOperation("AttachmentsIdPut")]
        [SwaggerResponse(200, type: typeof(Attachment))]
        public virtual IActionResult AttachmentsIdPut([FromRoute]int id, [FromBody]Attachment item)
        {
            return this._service.AttachmentsIdPutAsync(id, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <response code="201">Attachment created</response>
        [HttpPost]
        [Route("/api/attachments")]
        [SwaggerOperation("AttachmentsPost")]
        [SwaggerResponse(200, type: typeof(Attachment))]
        public virtual IActionResult AttachmentsPost([FromBody]Attachment item)
        {
            return this._service.AttachmentsPostAsync(item);
        }
    }
}
