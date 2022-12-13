import React, { useEffect, useState } from "react";
import axios from "axios";
import styled from "styled-components";
import Loader from "react-loader-spinner";
import NumberFormat from "react-number-format";

import { Formik, Field, Form } from "formik";
import { TextField as FormikTextField, CheckboxWithLabel as FormikCheckbox } from "formik-material-ui";

import Checkbox from "@material-ui/core/Checkbox";
import Chip from "@material-ui/core/Chip";
import { Dialog, Fade, makeStyles, MenuItem } from "@material-ui/core";
import IconButton from "@material-ui/core/IconButton";
import Modal from "@material-ui/core/Modal";
import Paper from "@material-ui/core/Paper";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";

import EditIcon from "@material-ui/icons/Edit";

import endpoints from "../../api-service/endpoints";
import {
  MONEYFORMATTER,
  PRIMARYCOLOR,
  PRIMARYBLUE,
  PRIMARYRED,
} from "../../constants/utils";
import { InverlevyButton } from "../InvyButton/InverlevyButton.component";

const NumberFormatCustom = (props) => {
  const { inputRef, onChange, ...other } = props;
  return (
    <NumberFormat
      {...other}
      getInputRef={inputRef}
      onValueChange={(values) => {
        onChange({
          target: {
            name: props.name,
            value: values.value,
          },
        });
      }}
      thousandSeparator
      isNumericString
      prefix="$"
    />
  );
};

const useStyles = makeStyles((theme) => ({
  table: {
    minWidth: 650,
  },
}));

const StyledForm = styled(Form)`
  display: grid;
  grid-gap: 1em;
  width: 20em;
  align-items: center;
`;

const UnitList = (props) => {
  let [units, setUnits] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [editUnit, setEditUnit] = useState("");
  const [open, setOpen] = useState(false);

  const classes = useStyles();

  const unitStatus = [
    { name: "Disponible", value: 0 },
    { name: "Apartado", value: 1 },
    { name: "Vendido", value: 2 },
  ];

  var getUnits = async () => {
    let { data } = await axios.get(`${endpoints.PROJECT}/${props.id}/units`);

    // Sort units alphabetically by name
    data = data.sort((a, b) => a.level - b.level || a.number - b.number);

    setUnits(data);
    setIsLoading(false);
  };

  const handleClose = () => {
    getUnits();
    setOpen(false);
  };
  const handleOpen = (row) => {
    setOpen(true);
    setEditUnit(row);
  };

  useEffect(() => {
    getUnits();
  }, []);

  if (isLoading) {
    return (
      <Paper>
        <div
          style={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <Loader type="ThreeDots" color={PRIMARYCOLOR} />
        </div>
      </Paper>
    );
  }

  const body = (
    <Dialog open={open} onClose={handleClose}>
      <Fade in={open}>
        <Paper style={{ padding: 32 }}>
          <h2 id="simple-modal-title">Editar Unidad</h2>
          <Formik
            initialValues={{
              estatus: editUnit.status || 0,
              precio: editUnit.price || "",
            }}
            onSubmit={async (values) => {
              try {
                await axios.put(`${endpoints.UNIT}/${editUnit.id}`, {
                  number: editUnit.number,
                  // level: editUnit.level,
                  // grossArea: editUnit.grossArea,
                  // builtUpArea: editUnit.builtUpArea,
                  // terraceArea: editUnit.terraceArea,
                  status: values.estatus,
                  price: Number(values.precio),
                  // isActive: editUnit.isActive
                });
                props.setSuccess(true);
              } catch (error) {
                console.log(error);
                props.setError(true);
              } finally {
                handleClose();
              }
            }}
          >
            {({ errors, isSubmitting, values }) => (
              <StyledForm>
                <Field
                  component={FormikTextField}
                  name="precio"
                  variant="outlined"
                  type="text"
                  label="Precio *"
                  InputProps={{
                    autoComplete: "off",
                    inputComponent: NumberFormatCustom,
                  }}
                />
                <Field
                  component={FormikTextField}
                  label="Estatus *"
                  type="text"
                  name="estatus"
                  variant="outlined"
                  select={true}
                >
                  {unitStatus.map((status) => (
                    <MenuItem value={status.value} key={status.value}>
                      {status.name}
                    </MenuItem>
                  ))}
                </Field>
                {isSubmitting ? (
                  <Loader
                    style={{ display: "flex", justifyContent: "center" }}
                    type="ThreeDots"
                    color={PRIMARYCOLOR}
                  />
                ) : (
                  <InverlevyButton type="submit" disabled={isSubmitting}>
                    {" "}
                    Guardar cambios
                  </InverlevyButton>
                )}
              </StyledForm>
            )}
          </Formik>
        </Paper>
      </Fade>
    </Dialog>
  );

  return (
    <>
      <Modal
        open={open}
        onClose={handleClose}
        aria-labelledby="simple-modal-title"
        aria-describedby="simple-modal-description"
      >
        {body}
      </Modal>

      <TableContainer component={Paper}>
        <Table className={classes.table} aria-label="simple table">
          <TableHead>
            <TableRow>
              <TableCell align="left">Nombre</TableCell>
              <TableCell align="center">Nivel</TableCell>
              <TableCell align="right">Precio</TableCell>
              <TableCell align="right">Valor Mínimo Esperado</TableCell>
              <TableCell align="center">Área de Terreno</TableCell>
              <TableCell align="center"></TableCell>
              <TableCell align="center">Estatus</TableCell>
              <TableCell align="center">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {units.map((row) => (
              <TableRow key={row.id}>
                <TableCell align="left">{row.displayName}</TableCell>
                <TableCell align="right">
                  {row.level > 0 ? row.level : "PB"}
                </TableCell>
                <TableCell align="right">
                  {MONEYFORMATTER.format(row.price)}
                </TableCell>
                <TableCell align="right">
                  {MONEYFORMATTER.format(row.minimumExpectedValue)}
                </TableCell>
                <TableCell align="right">
                  {row.grossArea} <small>&#13217;</small>
                </TableCell>
                <TableCell align="center">
                  {row.isCondoHotel && (
                    <Checkbox
                      style={{ color: PRIMARYCOLOR, userSelect: "none" }}
                      disabled={true}
                      checked={true}
                    />
                  )}
                </TableCell>
                <TableCell align="center">
                  {row.status === 0 ? (
                    <Chip
                      color="primary"
                      style={{ backgroundColor: PRIMARYBLUE }}
                      label="Disponible"
                    />
                  ) : row.status === 1 ? (
                    <Chip
                      color="primary"
                      style={{ backgroundColor: PRIMARYRED }}
                      label="Apartado"
                    />
                  ) : (
                    <Chip label="Vendido" />
                  )}
                </TableCell>
                <TableCell align="center">
                  <IconButton
                    size="small"
                    aria-label="edit"
                    onClick={() => {
                      handleOpen(row);
                    }}
                  >
                    <EditIcon />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </>
  );
};

export default UnitList;
