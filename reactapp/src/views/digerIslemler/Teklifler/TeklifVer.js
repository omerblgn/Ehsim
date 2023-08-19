import { Button, Container, FormControl, Grid, InputAdornment, LinearProgress, TextField } from '@mui/material';
import React from 'react';
import { useState } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import { useLocation, useNavigate, useParams } from 'react-router-dom';
import { useEffect } from 'react';

const sleep = (delay) => new Promise((resolve) => setTimeout(resolve, delay));

function TeklifVer() {
    const navigate = useNavigate();
    const { id: urunId } = useParams();
    const location = useLocation();

    const [fetchingError, setFetchingError] = useState(false);
    const [isFetching, setIsFetching] = useState(false);
    const [isUpdate, setIsUpdate] = useState(0);
    const [urunAdi, setUrunAdi] = useState('');
    const [teklifDegeri, setTeklifDegeri] = useState('');
    const [urunSahibi, setUrunSahibi] = useState('');
    const [validationErrors, setValidationErrors] = React.useState({});

    useEffect(() => {
        if (location.state != null) {
            setUrunAdi(location.state.urunAdi);
            setUrunSahibi(location.state.urunSahibi);
        }
    }, [urunId]);

    // useEffect(() => {
    //     console.log('Ürün Id: ' + urunId);
    //     if (typeof urunId !== 'undefined') {
    //         setIsUpdate(urunId);
    //         setIsFetching(true);
    //         teklifGetirPromise();
    //     } else {
    //         setUrunAdi('');
    //         setTeklifDegeri('');
    //         setMusteriAdi('');
    //         setIsFetching(false);
    //     }
    // }, [urunId]);

    // const teklifGetirPromise = () => {
    //     return new Promise(async (resolve, reject) => {
    //         const start = Date.now();
    //         setValidationErrors({});
    //         let config = {
    //             method: 'post',
    //             maxBodyLength: Infinity,
    //             url: 'http://localhost:5273/api/Teklif/Get',
    //             headers: {
    //                 'Content-Type': 'application/json',
    //                 Accept: 'text/plain'
    //             },
    //             params: {
    //                 id: urunId
    //             }
    //         };

    //         axios
    //             .request(config)
    //             .then(async (response) => {
    //                 console.log(JSON.stringify(response.data));
    //                 if (response.data.result) {
    //                     const millis = Date.now() - start;
    //                     if (millis < 500) {
    //                         await sleep(500 - millis);
    //                     }

    //                     setMusteriAdi(response.data.data.urunSahibi);
    //                     setTeklifDegeri(response.data.data.teklifDegeri);
    //                     setUrunAdi(response.data.data.urunAdi);
    //                     setFetchingError(false);
    //                     resolve(response.data); // Başarılı sonuç d1urumunda Promise'ı çöz
    //                 } else {
    //                     setFetchingError(true);
    //                     reject(new Error('İşlem başarısız')); // Başarısız sonuç durumunda Promise'ı reddet
    //                 }
    //             })
    //             .catch((error) => {
    //                 setFetchingError(true);
    //                 console.log(error);
    //                 reject(error); // Hata durumunda Promise'ı reddet
    //             })
    //             .finally(() => {
    //                 setIsFetching(false);
    //             });
    //     });
    // };

    const teklifVer = () => {
        // if (typeof urunId !== 'undefined') {
        //     toast.promise(teklifVerPromise, {
        //         pending: 'Teklif güncelleniyor',
        //         success: 'Teklif ' + teklifDegeri + '₺ olarak güncellendi 👌',
        //         error: 'Teklif güncellenirken hata oluştu 🤯'
        //     });
        // } else {
        toast.promise(teklifVerPromise, {
            pending: 'Teklif veriliyor',
            success: teklifDegeri + '₺ değerinde teklif verildi 👌',
            error: 'Teklif verilirken hata oluştu 🤯'
        });
        // }
    };

    const teklifVerPromise = () => {
        return new Promise(async (resolve, reject) => {
            const start = Date.now();
            setValidationErrors({});
            let data = JSON.stringify({
                urunId: urunId,
                musteriId: 1, //sadece bir tane müşteri olduğu için otomatik olarak o müşterinin id'si (1) veriliyor
                teklifDegeri: teklifDegeri
            });

            let config = {
                method: 'post',
                maxBodyLength: Infinity,
                url: 'http://localhost:5273/api/Teklif/Create',
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
                        navigate(`/digerIslemler/urunler`);
                        resolve(response.data); // Başarılı sonuç durumunda Promise'ı çöz
                    } else {
                        reject(new Error('İşlem başarısız')); // Başarısız sonuç durumunda Promise'ı reddet
                    }
                })
                .catch((error) => {
                    console.log(error);
                    setValidationErrors(error.response.data.errors);
                    reject(error); // Hata durumunda Promise'ı reddet
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
                                <Button onClick={teklifVer} className="mb-2" margin="normal" variant="contained">
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

export default TeklifVer;
