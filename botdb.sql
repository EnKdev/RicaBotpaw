-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server Version:               5.7.20-log - MySQL Community Server (GPL)
-- Server Betriebssystem:        Win32
-- HeidiSQL Version:             9.4.0.5125
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Exportiere Datenbank Struktur für botdb
CREATE DATABASE IF NOT EXISTS `botdb` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `botdb`;

-- Exportiere Struktur von Tabelle botdb.discord
CREATE TABLE IF NOT EXISTS `discord` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` varchar(50) DEFAULT NULL,
  `username` varchar(50) DEFAULT NULL,
  `tokens` int(10) unsigned NOT NULL DEFAULT '0',
  `customRank` varchar(50) DEFAULT '',
  `level` int(11) DEFAULT '1',
  `xp` int(11) DEFAULT '1',
  `daily` datetime NOT NULL DEFAULT '2001-01-01 00:00:00',
  `guid` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle botdb.moneydiscord
CREATE TABLE IF NOT EXISTS `moneydiscord` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` varchar(50) DEFAULT NULL,
  `money` int(11) NOT NULL DEFAULT '0',
  `storeMoney` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;

-- Daten Export vom Benutzer nicht ausgewählt
-- Exportiere Struktur von Tabelle botdb.sonatable
CREATE TABLE IF NOT EXISTS `sonatable` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` varchar(50) DEFAULT NULL,
  `sonaname` varchar(50) DEFAULT NULL,
  `age` int(11) NOT NULL,
  `species` varchar(50) DEFAULT NULL,
  `gender` varchar(50) DEFAULT NULL,
  `sexuality` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

-- Daten Export vom Benutzer nicht ausgewählt
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
