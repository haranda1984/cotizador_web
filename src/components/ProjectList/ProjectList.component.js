import React, { useEffect, useState } from "react";
import axios from "axios";
import * as moment from "moment";
import styled from "styled-components";
import Loader from "react-loader-spinner";

import { Formik, Field, Form } from "formik";
import { TextField as FormikTextField } from "formik-material-ui";
import { DatePicker as FormikDatePicker } from "formik-material-ui-pickers";

import { Dialog, Fade, makeStyles, MenuItem } from "@material-ui/core";
import Box from "@material-ui/core/Box";
import Checkbox from "@material-ui/core/Checkbox";
import Collapse from "@material-ui/core/Collapse";
import IconButton from "@material-ui/core/IconButton";
import InputAdornment from "@material-ui/core/InputAdornment"
import Modal from "@material-ui/core/Modal";
import Paper from "@material-ui/core/Paper";
import Snackbar from "@material-ui/core/Snackbar";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import Typography from "@material-ui/core/Typography";

import EditIcon from "@material-ui/icons/Edit";
import KeyboardArrowDownIcon from "@material-ui/icons/KeyboardArrowDown";
import KeyboardArrowUpIcon from "@material-ui/icons/KeyboardArrowUp";

import MuiAlert from "@material-ui/lab/Alert";

import endpoints from "../../api-service/endpoints";
import { PRIMARYCOLOR } from "../../constants/utils";
import TradePolicyList from "../TradePolicyList/TradePolicyList.component";
import UnitList from "../UnitList/UnitList.component";
import { InverlevyButton } from "../InvyButton/InvyButton.component";

const StyledForm = styled(Form)`
  display: grid;
  grid-gap: 1em;
  width: 20em;
  align-items: center;
`;

function Alert(props) {
  return <MuiAlert elevation={6} variant="filled" {...props} />;
}

const useRowStyles = makeStyles({
  root: {
    "& > *": {
      borderBottom: "unset",
    },
  },
});

const ProjectListRow = ({ row, handleClose, handleOpen }) => {
  const [openUnits, setOpenUnits] = useState(false);
  const [openTradingPolicies, setOpenTradingPolicies] = useState(false);
  const classes = useRowStyles();

  return (
    <React.Fragment>
      <TableRow className={classes.root}>
        <TableCell align="left">{row.displayName}</TableCell>
        <TableCell align="left">{row.description}</TableCell>
        <TableCell align="left">
          {moment(row.deliveryDate).format("yyyy-MM-DD")}
        </TableCell>
        <TableCell align="center">{row.currentStage}</TableCell>
        <TableCell align="center">
          {(row.allowsCondoHotel) && <Checkbox
            style={{ color: PRIMARYCOLOR, userSelect: 'none' }}
            disabled={true}
            checked={true}
          />}
        </TableCell>
        <TableCell align="center">
          <IconButton
            aria-label="expand row"
            size="small"
            onClick={() => {
              setOpenUnits(!openUnits);
              setOpenTradingPolicies(false);
            }}
          >
            {row.unitsNumber}
            {openUnits ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
          </IconButton>
        </TableCell>
        <TableCell align="center">
          <IconButton
            aria-label="expand row"
            size="small"
            onClick={() => {
              setOpenTradingPolicies(!openTradingPolicies);
              setOpenUnits(false);
            }}
          >
            {row.tradePoliciesNumber}
            {openTradingPolicies ? (
              <KeyboardArrowUpIcon />
            ) : (
              <KeyboardArrowDownIcon />
            )}
          </IconButton>
        </TableCell>
        <TableCell align="center">
          <IconButton
            size="small"
            aria-label="edit"
            onClick={() => {
              handleOpen();
            }}
          >
            <EditIcon />
          </IconButton>
        </TableCell>
      </TableRow>
      <TableRow>
        <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={8}>
          <Collapse in={openUnits} timeout="auto" unmountOnExit>
            <Box margin={(5, 5)}>
              <Typography variant="h6" gutterBottom component="div">
                Unidades
              </Typography>
              <UnitList id={row.id} />
            </Box>
          </Collapse>
        </TableCell>
      </TableRow>
      <TableRow>
        <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={8}>
          <Collapse in={openTradingPolicies} timeout="auto" unmountOnExit>
            <Box margin={(5, 5)}>
              <Typography variant="h6" gutterBottom component="div">
                Politicas
              </Typography>
              <TradePolicyList id={row.id} />
            </Box>
          </Collapse>
        </TableCell>
      </TableRow>
    </React.Fragment>
  );
};

const ProjectList = () => {
  const [projects, setProjects] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  const [updateProject, setUpdateProject] = useState("");
  const [open, setOpen] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState(false);

  const handleClose = () => {
    getProject();
    setOpen(false);
  };
  const handleAlertClose = () => setError(false);
  const handleSuccessClose = () => setSuccess(false);

  const getProject = async () => {
    let { data } = await axios.get(endpoints.PROJECT);
    data = [...data].filter((x) => x.isActive !== false);

    // Sort projects alphabetically by name
    data = data.sort((a, b) =>
      a.displayName < b.displayName ? -1 : a.displayName > b.displayName ? 1 : 0
    );

    setProjects(data);
    setIsLoading(false);
  };

  useEffect(() => {
    getProject();
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

  const classes = makeStyles((theme) => ({
    modal: {
      display: "flex",
      alignItems: "center",
      justifyContent: "center",
    },
    paper: {
      position: "absolute",
      backgroundColor: theme.palette.background.paper,
      border: "2px solid #000",
      boxShadow: theme.shadows[5],
      padding: theme.spacing(2, 4, 3),
    },
  }));

  const body = (
    <Dialog open={open} onClose={handleClose} className={classes.modal}>
      <Fade in={open}>
        <div style={{ padding: 32 }} className={classes.paper}>
          <h2 id="simple-modal-title">Editar Proyecto</h2>
          <Formik
            initialValues={{
              nombre: updateProject.displayName || "",
              descripcion: updateProject.description || "",
              etapa: updateProject.currentStageId || "",
              fechaEntrega: new Date(updateProject.deliveryDate) || new Date(),
              capRate: updateProject.capRate * 100
            }}
            onSubmit={async (values) => {
              try {
                await axios.put(`${endpoints.PROJECT}/${updateProject.id}`, {
                  name: updateProject.name,
                  displayName: values.nombre,
                  description: values.descripcion,
                  photoUrl: updateProject.photoUrl,
                  themeColor: updateProject.themeColor,
                  location: updateProject.location,
                  deliveryDate: values.fechaEntrega,
                  currentStageId: values.etapa,
                  capRate: values.capRate / 100,
                });
                setSuccess(true);
              } catch (error) {
                console.log(error);
                setError(true);
              } finally {
                handleClose();
              }
            }}
          >
            {({ errors, isSubmitting, values }) => (
              <StyledForm>
                <Field
                  component={FormikTextField}
                  name="nombre"
                  variant="outlined"
                  type="text"
                  label="Nombre *"
                  InputProps={{
                    autoComplete: "off",
                  }}
                />
                <Field
                  component={FormikTextField}
                  name="descripcion"
                  variant="outlined"
                  type="text"
                  label="Descripción"
                  InputProps={{
                    autoComplete: "off",
                  }}
                />
                <Field
                  component={FormikDatePicker}
                  label="Fecha entrega *"
                  name="fechaEntrega"
                  inputVariant="outlined"
                  format="L"
                />
                <Field
                  component={FormikTextField}
                  label="Etapa *"
                  type="text"
                  name="etapa"
                  variant="outlined"
                  select={true}
                >
                  {updateProject.stages.map((stage) => (
                    <MenuItem value={stage.id} key={stage.id}>
                      {stage.name}
                    </MenuItem>
                  ))}
                </Field>
                <Field
                  component={FormikTextField}
                  name="capRate"
                  variant="outlined"
                  type="number"
                  label="Cap Rate"
                  className="no-spin"
                  inputProps={{
                    className: "no-spin",
                    max: 100,
                  }}
                  InputProps={{
                    endAdornment: <InputAdornment position="end">%</InputAdornment>,
                  }}
                />
                {isSubmitting ? (
                  <Loader
                    style={{ display: "flex", justifyContent: "center" }}
                    type="ThreeDots"
                    color={PRIMARYCOLOR}
                  />
                ) : (
                  <InvyButton type="submit" disabled={isSubmitting}>
                    {" "}
                    Guardar cambios
                  </InvyButton>
                )}
              </StyledForm>
            )}
          </Formik>
        </div>
      </Fade>
    </Dialog>
  );

  return (
    <>
      <Snackbar
        open={error}
        autoHideDuration={10000}
        onClose={handleAlertClose}
      >
        <Alert onClose={handleAlertClose} severity="error">
          Ocurrió un error, por favor intente de nuevo. Si el error persiste
          contacte{" "}
          <a href="mailto:soporte@rootinc.mx">
            soporte@rootinc.mx{" "}
          </a>
        </Alert>
      </Snackbar>
      <Snackbar
        open={success}
        autoHideDuration={5000}
        onClose={handleSuccessClose}
      >
        <Alert onClose={handleSuccessClose} severity="success">
          Operación realizada con éxito.
        </Alert>
      </Snackbar>
      <Modal
        open={open}
        onClose={handleClose}
        aria-labelledby="simple-modal-title"
        aria-describedby="simple-modal-description"
      >
        {body}
      </Modal>

      <TableContainer component={Paper}>
        <Table aria-label="collapsible table">
          <TableHead>
            <TableRow>
              <TableCell align="left">Nombre</TableCell>
              <TableCell align="left">Descripción</TableCell>
              <TableCell align="left">Fecha entrega</TableCell>
              <TableCell align="center">Etapa</TableCell>
              <TableCell align="center"> </TableCell>
              <TableCell align="center">Unidades</TableCell>
              <TableCell align="center">Políticas</TableCell>
              <TableCell align="center">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {projects.map((row) => (
              <ProjectListRow
                row={row}
                key={row.id}
                open={open}
                handleClose={() => {
                  handleClose();
                }}
                handleOpen={async () => {
                  setOpen(true);
                  setUpdateProject(row);
                }}
              />
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </>
  );
};

export default ProjectList;
