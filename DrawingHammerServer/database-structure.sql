/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

DROP DATABASE IF EXISTS `drawing-hammer`;
CREATE DATABASE IF NOT EXISTS `drawing-hammer` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `drawing-hammer`;

DROP TABLE IF EXISTS `users`;
CREATE TABLE IF NOT EXISTS `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(64) NOT NULL,
  `password` char(64) NOT NULL,
  `isBanned` tinyint(1) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `user_salt`;
CREATE TABLE IF NOT EXISTS `user_salt` (
  `userID` int(11) NOT NULL,
  `salt` varchar(250) NOT NULL,
  PRIMARY KEY (`userID`),
  CONSTRAINT `FK_user_salt_users` FOREIGN KEY (`userID`) REFERENCES `users` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

DROP TABLE IF EXISTS `words`;
CREATE TABLE IF NOT EXISTS `words` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `word` varchar(50) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*!40000 ALTER TABLE `words` DISABLE KEYS */;
INSERT INTO `words` (`ID`, `word`) VALUES
	(1, 'Kaffeemaschine'),
	(2, 'Wasserwaage'),
	(3, 'Rucksack'),
	(4, 'Messerblock'),
	(5, 'Spülmaschine'),
	(6, 'Tiefgarage'),
	(7, 'Helikopter'),
	(8, 'Fußballstadion'),
	(9, 'Handy'),
	(10, 'Tastatur'),
	(11, 'Sonnenuntergang'),
	(12, 'Jahreszeiten'),
	(13, 'Nacht'),
	(14, 'Rollkragenpullover'),
	(15, 'Unterwäsche'),
	(16, 'Bus'),
	(17, 'Bahnhof'),
	(18, 'Autobahnausfahrt'),
	(19, 'Polizeikontrolle'),
	(20, 'Datenbank'),
	(21, 'Decke'),
	(22, 'Sattelitenschüssel'),
	(23, 'Salatschleuder'),
	(24, 'Kalender'),
	(25, 'Auflaufform'),
	(26, 'Serverschrank'),
	(27, 'Fernbedienung'),
	(28, 'Perrücke'),
	(29, 'Beerdigung'),
	(30, 'Geldbeutel'),
	(31, 'Monitor'),
	(32, 'Schreibtischstuhl'),
	(33, 'Einkaufswagen'),
	(34, 'Verkehrspolizist'),
	(35, 'Fahrschule'),
	(36, 'Prüfung'),
	(38, 'Arbeitsmoral'),
	(39, 'Schokonikolaus'),
	(40, 'Schlitten'),
	(41, 'Badehose'),
	(42, 'Muskelkater'),
	(43, 'Fitnesstudio'),
	(44, 'Abschlussarbeit'),
	(45, 'Wartezimmer'),
	(46, 'Zahnarztpraxis'),
	(47, 'Marathonläufer'),
	(49, 'Urlaub'),
	(50, 'Flugzeug');
/*!40000 ALTER TABLE `words` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;


/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
