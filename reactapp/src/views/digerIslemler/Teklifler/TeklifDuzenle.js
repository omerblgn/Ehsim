import { Button, Container, FormControl, Grid, InputAdornment, LinearProgress, TextField } from '@mui/material';
import React from 'react';
import { useState } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { useLocation, useNavigate, useParams } from 'react-router-dom';
import { useEffect } from 'react';

const sleep = (delay) => new Promise((resolve) => setTimeout(resolve, delay));

function TeklifDuzenle() {
    const navigate = useNavigate();
    const { id } = useParams();
    const location = useLocation();

    const [fetchingError, setFetchingError] = useState(false);
    const [isFetching, setIsFetching] = useState(false);
    const [isUpdate, setIsUpdate] = useState(0);
    const [urunId, setUrunId] = useState('');
    const [urunAdi, setUrunAdi] = useState('');
    const [teklifDegeri, setTeklifDegeri] = useState('');
    const [urunSahibi, setUrunSahibi] = useState('');
    const [validationErrors, setValidationErrors] = React.useState({});

    useEffect(() => {
        if (location.state != null) {
            setUrunAdi(location.state.urunAdi);
            setUrunSahibi(location.state.urunSahibi);
            setIsUpdate(id);
            setIsFetching(true);
            teklifGetirPromise();
        }
    }, [id]);

    const teklifGetirPromise = () => {
        return new Promise(async (resolve, reject) => {
            const start = Date.now();
            setValidationErrors({});
            let config = {
                method: 'post',
                maxBodyLength: Infinity,
                url: 'http://localhost:5273/api/Teklif/Get',
                headers: {
                    'Content-Type': 'application/json',
                    Accept: 'text/plain'
                },
                params: {
                    id: id
                }
            };

            axios
                .request(config)
                .then(async (response) => {
                    console.log(JSON.stringify(response.data));
                    if (response.data.result) {
                        const millis = Date.now() - start;
                        if (millis < 500) {
                            await sleep(500 - millis);
                        }

                        setUrunSahibi(response.data.data.urunSahibi);
                        setTeklifDegeri(response.data.data.teklifDegeri);
                        setUrunAdi(response.data.data.urunAdi);
                        setUrunId(response.data.data.urunId);
                        setFetchingError(false);

                        resolve(response.data); // Başarılı sonuç d1urumunda Promise'ı çöz
                    } else {
                        setFetchingError(true);
                        reject(new Error('İşlem başarısız')); // Başarısız sonuç durumunda Promise'ı reddet
                    }
                })
                .catch((error) => {
                    setFetchingError(true);
                    console.log(error);
                    reject(error); // Hata durumunda Promise'ı reddet
                })
                .finally(() => {
                    setIsFetching(false);
                });
        });
    };

    const teklifDuzenle = () => {
        toast.promise(teklifDuzenlePromise, {
            pending: 'Teklif güncelleniyor',
            success: 'Teklif ' + teklifDegeri + '₺ olarak güncellendi 👌',
            error: 'Teklif güncellenirken hata oluştu 🤯'
        });
    };

    const teklifDuzenlePromise = () => {
        return new Promise(async (resolve, reject) => {
            const start = Date.now();
            setValidationErrors({});
            let data = JSON.stringify({
                id: id,
                urunId: urunId,
                musteriId: 1, //sadece bir tane müşteri olduğu için otomatik olarak o müşterinin id'si (1) veriliyor
                teklifDegeri: teklifDegeri
            });

            let config = {
                method: 'post',
                maxBodyLength: Infinity,
                url: 'http://localhost:5273/api/Teklif/Update',
                headers: {
                    'Content-Type': 'application/json',
                    Accept: 'text/plain'
                },
                data: data
            };

            axios
                .request(config)
                .then(async (response) => {
                    console.log(JSON.stringify(response.data));
                    if (response.data.result) {
                        const millis = Date.now() - start;
                        if (millis < 700) {
                            await sleep(700 - millis);
                        }
                        navigate(`/digerIslemler/tekliflerim`);
                        resolve(response.data);
                    } else {
                        reject(new Error('İşlem başarısız'));
                    }
                })
                .catch((error) => {
                    console.log(error);
                    setValidationErrors(error.response.data.errors);
                    reject(error);
                });
        });
    };

    return (
        <>
            <Container className="d-flex justify-content-center" maxWidth="md">
                <Grid item xs={6}>
                    <FormControl sx={{ m: 0, width: '50ch' }}>
                        {isFetching && <LinearProgress className="mt-3" color="secondary" />}
                        {(isUpdate === 0 || !isFetching) && (
                            <>
                                <TextField
                                    disabled
                                    required
                                    margin="normal"
                                    id="urunAdi"
                                    label="Ürün Adı"
                                    variant="outlined"
                                    value={urunAdi}
                                    InputLabelProps={{ shrink: true }}
                                />
                                <TextField
                                    disabled
                                    required
                                    margin="normal"
                                    id="urunSahibi"
                                    label="Ürün Sahibi"
                                    variant="outlined"
                                    value={urunSahibi}
                                    InputLabelProps={{ shrink: true }}
                                />
                                <TextField
                                    required
                                    margin="normal"
                                    id="teklifDegeri"
                                    label="Teklif"
                                    variant="outlined"
                                    value={teklifDegeri}
                                    InputLabelProps={{ shrink: true }}
                                    onChange={(e) => setTeklifDegeri(e.target.value)}
                                    InputProps={{
                                        startAdornment: <InputAdornment position="end">₺</InputAdornment>
                                    }}
                                    type="number"
                                    error={!!validationErrors.dataVM}
                                    helperText={validationErrors.dataVM && 'Teklif boş bırakılamaz.'}
                                />
                                <Button onClick={teklifDuzenle} className="mb-2" margin="normal" variant="contained">
                                    Kaydet
                                </Button>
                            </>
                        )}
                    </FormControl>
                </Grid>
            </Container>
        </>
    );
}

export default TeklifDuzenle;
