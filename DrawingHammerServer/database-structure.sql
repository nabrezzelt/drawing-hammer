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

/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` (`id`, `username`, `password`, `isBanned`) VALUES
	(1, '123456', '2AAA9630D41B3E3050EDBD899669BA48A94AB86E47ECD22CFDD5A6F868AB720D', 0),
	(2, '654321', '55D9D475B997A77DB1BEF96458BF4218AA9070A05EBC2DC167AFF13C07E21027', 0),
	(3, 'qwertz', 'D4228FBB276695171685771570B199221BE0F2AAB22D8F1FB94AC8D478CF7DDE', 0),
	(4, 'asdfgh', '3DE2BFF1C461478EDFF6495268C07B075989E8CA7C7167013F9C30949197E9A6', 0);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;

CREATE TABLE IF NOT EXISTS `user_salt` (
  `userID` int(11) NOT NULL,
  `salt` varchar(250) NOT NULL,
  PRIMARY KEY (`userID`),
  CONSTRAINT `FK_user_salt_users` FOREIGN KEY (`userID`) REFERENCES `users` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Exportiere Daten aus Tabelle drawing-hammer.user_salt: ~4 rows (ungefähr)
/*!40000 ALTER TABLE `user_salt` DISABLE KEYS */;
INSERT INTO `user_salt` (`userID`, `salt`) VALUES
	(1, 'VSs6Tc2hy7nceAVFiPXbQO3QA0Ox5rebGXm5PXqhUqp1eesYuUiLKsIbg6Ik0'),
	(2, 'EU9HQHTUf0kpAqRd9p3tJg3CE7GkgYIszWOMgyAhSEjfdwITfGmhRvnVUPUMXziE'),
	(3, 'saBw3S4GWXGXxoJm3Zq4gwSVnmnD7dfgttBBLy4bJU5E1SnNvUXSoFtnvGy50zcDU'),
	(4, 'cln1EiqkL0XUYK7U5k1nwcD4E4SvOby3TQldc1DZIyeAz6vViF1xoqKKH1ihWjSQ');
/*!40000 ALTER TABLE `user_salt` ENABLE KEYS */;



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
INSERT INTO `words` (`word`) VALUES
	('Kaffeemaschine'),
	('Wasserwaage'),
	('Rucksack'),
	('Messerblock'),
	('Spülmaschine'),
	('Tiefgarage'),
	('Helikopter'),
	('Fußballstadion'),
	('Handy'),
	('Tastatur'),
	('Sonnenuntergang'),
	('Jahreszeiten'),
	('Nacht'),
	('Rollkragenpullover'),
	('Unterwäsche'),
	('Bus'),
	('Bahnhof'),
	('Autobahnausfahrt'),
	('Polizeikontrolle'),
	('Datenbank'),
	('Decke'),
	('Sattelitenschüssel'),
	('Salatschleuder'),
	('Kalender'),
	('Auflaufform'),
	('Serverschrank'),
	('Fernbedienung'),
	('Perrücke'),
	('Beerdigung'),
	('Geldbeutel'),
	('Monitor'),
	('Schreibtischstuhl'),
	('Einkaufswagen'),
	('Verkehrspolizist'),
	('Fahrschule'),
	('Prüfung'),
	('Arbeitsmoral'),
	('Schokonikolaus'),
	('Schlitten'),
	('Badehose'),
	('Muskelkater'),
	('Fitnesstudio'),
	('Abschlussarbeit'),
	('Wartezimmer'),
	('Zahnarztpraxis'),
	('Marathonläufer'),
	('Urlaub'),
	('Flugzeug');
/*!40000 ALTER TABLE `words` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;


/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
