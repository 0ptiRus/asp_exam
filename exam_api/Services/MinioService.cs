using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace exam_api.Services;

public enum MinioBuckets
{
    gallerybucketimages,
    gallerybucketvideos,
}

public class MinioService
{
        private readonly IMinioClient minio;
        private readonly ILogger logger;

        public MinioService(IConfiguration config, ILogger<MinioService> logger)
        {
            this.logger = logger;
            string endpoint, accessKey, secretKey;
            
                endpoint = config["Minio:Endpoint"];
                accessKey = config["Minio:AccessKey"];
                secretKey = config["Minio:SecretKey"];
            

            minio = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(false)
                .Build(); 
        }

        public async Task UploadFileAsync(string object_name, Stream data, string content_type, string bucket_name)
        {
            try
            {
                logger.LogInformation($"Looking for bucket with name {bucket_name}");
                
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket_name));
                if (!found)
                {
                    logger.LogWarning($"Bucket not found with name {bucket_name}, creating it");
                    await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket_name));
                    logger.LogInformation($"Bucket with name {bucket_name} created");
                }

                logger.LogInformation($"Putting object in bucket {bucket_name} with name {object_name}, " +
                                      $"content type - {content_type}, size - {data.Length}");
                await minio.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(bucket_name)
                    .WithObject(object_name)
                    .WithStreamData(data)
                    .WithObjectSize(data.Length)
                    .WithContentType(content_type));
                
                logger.LogInformation($"Object in bucket {bucket_name} with name {object_name} created");
            }
            catch (MinioException e)
            {
                logger.LogError($"MinIO ERROR {e.Message}");
            }
        }

        public async Task<string> GetFileUrlAsync(string object_name, string bucket_name)
        {
            try
            {
                logger.LogInformation($"Getting object in bucket {bucket_name} with name {object_name}");
                var args = new PresignedGetObjectArgs()
                    .WithBucket(bucket_name)
                    .WithObject(object_name)
                    .WithExpiry(3600); // 1 hour
                logger.LogInformation($"Created link for object with name {object_name} in bucket {bucket_name}, expiry - 1h");
                return await minio.PresignedGetObjectAsync(args);
            }
            catch (MinioException e)
            {
                logger.LogError($"[MinIO ERROR] {e.Message}");
                return null;
            }
        }

        public async Task DeleteFileAsync(string objectName, string bucket_name)
        {
            try
            {
                logger.LogInformation($"Deleting object in bucket {bucket_name} with name {objectName}");
                await minio.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(bucket_name).WithObject(objectName));
                logger.LogInformation($"Deleted object in bucket {bucket_name} with name {objectName}");
            }
            catch (MinioException e)
            {
                logger.LogError($"[MinIO ERROR] {e.Message}");
            }
        }
        
        public string GetBucketNameForFile(string content_type)
        {
            string[] imageMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp", "application/octet-stream" };
            string[] videoMimeTypes = new[] { "video/mp4", "video/webm", "video/ogg" };
            
            if (imageMimeTypes.Contains(content_type, StringComparer.OrdinalIgnoreCase))
            {
                return Enum.GetName(MinioBuckets.gallerybucketimages); 
            }

            if (videoMimeTypes.Contains(content_type, StringComparer.OrdinalIgnoreCase))
            {
                return Enum.GetName(MinioBuckets.gallerybucketvideos); 
            }
            
            return null;
        }
    }
