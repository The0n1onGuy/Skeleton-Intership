<?php
header('Content-Type: application/json');
$host = '192.250.227.13';
$db = 'clouddatacancun_dbs5349300';
$user = 'clouddatacancun_jtorres';
$pass = 'M0is3s2010';

try {
    $pdo = new PDO("mysql:host=$host;dbname=$db", $user, $pass, [
        PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION
    ]);

    $nombres = $_POST['nombres'] ?? '';
    $apellidos = $_POST['apellidos'] ?? '';
    $correo = $_POST['correo'] ?? '';
    $telefono = $_POST['telefono'] ?? '';
    $contrasena = $_POST['contrasena'] ?? '';

    $stmt = $pdo->prepare("INSERT INTO registro (nombres, apellidos, correo, telefono, contrasena) VALUES (?, ?, ?, ?, ?)");
    $stmt->execute([$nombres, $apellidos, $correo, $telefono, $contrasena]);

    $id = $pdo->lastInsertId();

    if (isset($_FILES['archivo_pdf']) && $_FILES['archivo_pdf']['error'] === UPLOAD_ERR_OK) {
        $pdf = $_FILES['archivo_pdf'];
        $datos = file_get_contents($pdf['tmp_name']);
        $stmt = $pdo->prepare("INSERT INTO archivos_sub (id_registro, archnombre, archtipo, archtam, archdatos) VALUES (?, ?, ?, ?, ?)");
        $stmt->execute([$id, $pdf['name'], $pdf['type'], $pdf['size'], $datos]);
    }

    if (isset($_FILES['archivo_img']) && $_FILES['archivo_img']['error'] === UPLOAD_ERR_OK) {
        $img = $_FILES['archivo_img'];
        $datos = file_get_contents($img['tmp_name']);
        $stmt = $pdo->prepare("INSERT INTO archivos_sub (id_registro, archnombre, archtipo, archtam, archdatos) VALUES (?, ?, ?, ?, ?)");
        $stmt->execute([$id, $img['name'], $img['type'], $img['size'], $datos]);
    }

    echo json_encode(["status" => "success", "message" => "Datos y archivos subidos", "id" => $id]);

} catch (PDOException $e) {
    echo json_encode(["status" => "error", "message" => $e->getMessage()]);
}
