-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: growsmart_db
-- ------------------------------------------------------
-- Server version	8.0.45

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20260320023241_InitialCreate','8.0.2');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `notifications`
--

DROP TABLE IF EXISTS `notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notifications` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `Title` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Message` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsRead` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `DueDate` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Notifications_UserId` (`UserId`),
  CONSTRAINT `FK_Notifications_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notifications`
--

LOCK TABLES `notifications` WRITE;
/*!40000 ALTER TABLE `notifications` DISABLE KEYS */;
INSERT INTO `notifications` VALUES (1,1,'Water','djdjdh','WATER',0,'2026-03-20 09:25:10.000000',NULL),(2,2,'Afaqrg','qtqfq','WATER',1,'2026-03-20 09:47:07.000000',NULL),(3,2,'Water','urgent','WATER',0,'2026-03-20 09:51:59.000000',NULL),(4,3,'Water ','jvjjgjf','HARVEST',0,'2026-03-21 17:58:14.000000',NULL),(5,2,'Water','urgent','WATER',0,'2026-03-21 18:41:59.000000',NULL);
/*!40000 ALTER TABLE `notifications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `passwordresetotps`
--

DROP TABLE IF EXISTS `passwordresetotps`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `passwordresetotps` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Email` varchar(255) NOT NULL,
  `OtpCode` varchar(255) NOT NULL,
  `ExpiryTime` datetime(6) NOT NULL,
  `IsUsed` tinyint(1) NOT NULL DEFAULT '0',
  `CreatedAt` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `passwordresetotps`
--

LOCK TABLES `passwordresetotps` WRITE;
/*!40000 ALTER TABLE `passwordresetotps` DISABLE KEYS */;
INSERT INTO `passwordresetotps` VALUES (11,'cms111573@gmail.com','498086','2026-04-02 17:00:07.341396',1,'2026-04-02 16:45:07.361467'),(13,'cms111573@gmail.com','385690','2026-04-03 10:59:42.697310',1,'2026-04-03 10:44:42.743428'),(14,'cms111573@gmail.com','637744','2026-04-07 03:34:42.652432',1,'2026-04-07 03:19:42.703550'),(15,'cms111573@gmail.com','300696','2026-04-07 06:36:50.716184',1,'2026-04-07 06:21:50.791051');
/*!40000 ALTER TABLE `passwordresetotps` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `plants`
--

DROP TABLE IF EXISTS `plants`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `plants` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Species` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Category` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `WaterFrequencyDays` int NOT NULL,
  `SunlightRequirement` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsIndoor` tinyint(1) NOT NULL,
  `DatePlanted` datetime(6) NOT NULL,
  `HealthStatus` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LastWateredAt` datetime DEFAULT NULL,
  `LastFertilizedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Plants_UserId` (`UserId`),
  CONSTRAINT `FK_Plants_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `plants`
--

LOCK TABLES `plants` WRITE;
/*!40000 ALTER TABLE `plants` DISABLE KEYS */;
INSERT INTO `plants` VALUES (1,1,'Tomatoes ','','',1,'Full Sun',0,'2026-03-20 00:00:00.000000','Healthy',NULL,NULL),(2,2,'Rose','','',3,'Full Sun',0,'2026-03-20 00:00:00.000000','Healthy','2026-03-21 12:09:42','2026-03-21 12:09:43'),(3,2,'Tomato','','',3,'Full Sun',0,'2026-03-20 00:00:00.000000','Healthy','2026-03-21 12:09:46','2026-03-21 12:09:48'),(4,2,'Tomato','','',1,'Full Sun',0,'2026-03-21 00:00:00.000000','Healthy','2026-03-21 12:09:45','2026-03-21 12:09:38'),(5,2,'Maggie','','',3,'Full Sun',0,'2026-03-22 00:00:00.000000','Healthy',NULL,NULL),(6,2,'Rose','','',1,'Full Sun',0,'2026-03-21 00:00:00.000000','Healthy',NULL,NULL),(9,7,'Rose','','',3,'Full Sun',0,'2026-03-31 00:00:00.000000','Healthy',NULL,NULL),(10,7,'Tomato','','',1,'Full Sun',0,'2026-03-31 00:00:00.000000','Healthy',NULL,NULL),(11,12,'Rose','','',1,'Full Sun',0,'2026-04-02 00:00:00.000000','Healthy',NULL,NULL);
/*!40000 ALTER TABLE `plants` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `profiles`
--

DROP TABLE IF EXISTS `profiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `profiles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `UserType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Location` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `GardenSize` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` longtext,
  `NotifMarket` tinyint(1) DEFAULT '0',
  `NotifWeather` tinyint(1) DEFAULT '1',
  `Address` longtext NOT NULL,
  `CropInterests` longtext NOT NULL,
  `FirstName` longtext NOT NULL,
  `LastName` longtext NOT NULL,
  `NotifCultivation` tinyint(1) NOT NULL DEFAULT '1',
  `ProfileImage` longtext NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Profiles_UserId` (`UserId`),
  CONSTRAINT `FK_Profiles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `profiles`
--

LOCK TABLES `profiles` WRITE;
/*!40000 ALTER TABLE `profiles` DISABLE KEYS */;
INSERT INTO `profiles` VALUES (1,1,'','','','',1,1,'','','','',1,''),(2,2,'Backyard','Chennai, India','Large','General',1,1,'','','','',1,''),(3,3,'Balcony','Chennai, India','Large','General',1,1,'','','','',1,''),(5,5,'','','','',1,1,'','','','',1,''),(7,7,'Backyard','Chennai, India','Large','General',1,1,'','','','',1,''),(8,8,'','','','',1,1,'','','','',1,''),(9,10,'Urban Gardener','Palanjur','Large','wreer',0,1,'ereere','Fruits,Vegetables','ms','naidu',1,''),(10,11,'Backyard','Chennai, India','Large','',0,1,'','General','','',1,''),(11,12,'Backyard','Chennai, India','Large','',0,1,'','General','','',1,''),(12,13,'Balcony','Chennai, India','Large','',0,1,'','General','','',1,'http://furlable-shaunta-catarrhal.ngrok-free.dev/Uploads/ProfileImages/aa89e3c5-82a3-455f-b228-acf68658d4fe_profile2247642061712981559.jpg'),(13,14,'Gardening Enthusiast','','','',0,1,'','','','',1,'');
/*!40000 ALTER TABLE `profiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Users_Email` (`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'farmer@gmail.com','madhu123','madhu sudhan','2026-03-20 03:00:47.852873'),(2,'janaki05@gmail.com','$2a$11$bp3r/94xxcxHArIA4iNyq.4gZvigTt/Ud0C8hbLWVQJawRv7a1Fsy','janaki','2026-03-20 04:06:39.654133'),(3,'nandi@gmail.com','1234567','nandi','2026-03-21 12:26:32.341195'),(5,'testuser@example.com','password123','Test User','2026-03-22 06:11:01.300959'),(7,'vaasudev23@gmail.com','$2a$11$7oNsRLo7KXdrlkYhJKplBeEpVzo8n8INqUrG8usB30XlqrNs3.Wcu','Sree Krishna','2026-03-23 16:31:23.323181'),(8,'naidu321@gmail.com','naidu321','naidu','2026-03-26 15:48:59.845162'),(9,'msnaidu12@gmail.com','msanidu','ms','2026-03-26 16:03:47.382210'),(10,'msnaidu13@gmail.com','msanidu','ms','2026-03-26 16:15:53.480542'),(11,'sudhan11@gmail.com','$2a$11$hwT8zPmZEWkiFHeAVa91oeRzBBLZd.3ANfhXR16dcqntJrvjhz5ci','Sudhan','2026-04-01 10:07:28.232788'),(12,'nandu@gmail.com','$2a$11$WuDzhQYyDWBWmcoWXUdIpeHQ/1yxMT37DCZ3roRP3hFeOrRxjxHPG','nandu','2026-04-02 08:51:16.675545'),(13,'cms111573@gmail.com','$2a$11$C6iapkqURJZHZ6fSPfdsre.tp6xSMUV/kd2O2M0UcaFZRB39JkJ1C','cms','2026-04-02 12:15:48.382731'),(14,'cmsnaidu@gmail.com','$2a$11$/XzX/uwPje.N8tl8gD3wm.t2WsOItabhmkoS91RnCza.ZEnY3MjtO','Madhu','2026-04-09 05:22:22.110323');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-04-09 15:23:51
