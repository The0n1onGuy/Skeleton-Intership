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
$operacion = $_POST['token_operacion'] ?? $_GET['token_operacion'] ?? 0;

    if ($operacion == 1) {
        $nombres = $_POST['nombres'] ?? '';
$apellidos = $_POST['apellidos'] ?? '';
$correo = $_POST['correo'] ?? '';
$telefono = $_POST['telefono'] ?? '';
$contrasena = $_POST['contrasena'] ?? '';
$usuario = $_POST['usuario'] ?? '';
$pais = $_POST['pais'] ?? '';
$estado = $_POST['estado'] ?? '';
$municipio = $_POST['municipio'] ?? '';
$codigoPostal = $_POST['codigopostal'] ?? '';


$stmt = $pdo->prepare("INSERT INTO registro (usuario, nombres, apellidos, correo, telefono, contrasena, pais, estado, municipio, Codigop) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)");
$stmt->execute([$usuario, $nombres, $apellidos, $correo, $telefono, $contrasena, $pais, $estado, $municipio, $codigoPostal]);

$id = $pdo->lastInsertId();

// PDF upload
if (isset($_FILES['archivo_pdf']) && $_FILES['archivo_pdf']['error'] === UPLOAD_ERR_OK) {
    $pdf = $_FILES['archivo_pdf'];
    $datos = file_get_contents($pdf['tmp_name']);
    $stmt = $pdo->prepare("INSERT INTO archivos_sub (id_registro, archnombre, archtipo, archtam, archdatos) VALUES (?, ?, ?, ?, ?)");
    $stmt->execute([$id, $pdf['name'], $pdf['type'], $pdf['size'], $datos]);
}

// First image
if (isset($_FILES['archivo_img1']) && $_FILES['archivo_img1']['error'] === UPLOAD_ERR_OK) {
    $img1 = $_FILES['archivo_img1'];
    $datos = file_get_contents($img1['tmp_name']);
    $stmt = $pdo->prepare("INSERT INTO archivos_sub (id_registro, archnombre, archtipo, archtam, archdatos) VALUES (?, ?, ?, ?, ?)");
    $stmt->execute([$id, $img1['name'], $img1['type'], $img1['size'], $datos]);
}

// Second image
if (isset($_FILES['archivo_img2']) && $_FILES['archivo_img2']['error'] === UPLOAD_ERR_OK) {
    $img2 = $_FILES['archivo_img2'];
    $datos = file_get_contents($img2['tmp_name']);
    $stmt = $pdo->prepare("INSERT INTO archivos_sub (id_registro, archnombre, archtipo, archtam, archdatos) VALUES (?, ?, ?, ?, ?)");
    $stmt->execute([$id, $img2['name'], $img2['type'], $img2['size'], $datos]);
}

    }

    else if ($operacion == 2) {
        // ---------- CASE 2: Consultar datos y archivos ----------
        $id = $_POST['id'] ?? $_GET['id'] ?? 0;

        if (!is_numeric($id) || $id <= 0) {
            echo json_encode(["status" => "error", "message" => "ID inválido"]);
            exit;
        }

        $stmt = $pdo->prepare("SELECT nombres, apellidos, correo, telefono, contrasena FROM registro WHERE id = ?");
        $stmt->execute([$id]);
        $user = $stmt->fetch(PDO::FETCH_ASSOC);

        if (!$user) {
            echo json_encode(["status" => "error", "message" => "Usuario no encontrado"]);
            exit;
        }

        $stmt = $pdo->prepare("SELECT id, archnombre, archtipo, archtam, uploaded_at FROM archivos_sub WHERE id_registro = ?");
        $stmt->execute([$id]);
        $files = [];
        
        while ($row = $stmt->fetch(PDO::FETCH_ASSOC)) {
            $files[] = [
                "id" => $row["id"],
                "archnombre" => $row["archnombre"],
                "archtipo" => $row["archtipo"],
                "archtam" => $row["archtam"],
                "uploaded_at" => $row["uploaded_at"]
            ];
        }

        echo json_encode([
            "status" => "success",
            "usuario" => $user,
            "archivos" => $files
        ]);
    }else if ($operacion == 3 && isset($_GET['file_id'])) {
    $fileId = intval($_GET['file_id']);

    $stmt = $pdo->prepare("SELECT archnombre, archtipo, archdatos FROM archivos_sub WHERE id = ?");
    $stmt->execute([$fileId]);
    $file = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$file) {
        http_response_code(404);
        exit("Archivo no encontrado");
    }

    header("Content-Type: " . $file['archtipo']);
    header("Content-Disposition: inline; filename=\"" . $file['archnombre'] . "\"");
    echo $file['archdatos'];
    exit;
}

    else {
        echo json_encode(["status" => "error", "message" => "Operación no válida"]);
    }

} catch (PDOException $e) {
    echo json_encode(["status" => "error", "message" => $e->getMessage()]);
}
